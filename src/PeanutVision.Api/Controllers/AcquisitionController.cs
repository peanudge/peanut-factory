using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Exceptions;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcquisitionController : ControllerBase
{
    private readonly IAcquisitionSession _session;
    private readonly IExposureController _exposure;
    private readonly ISnapshotCapture    _snapshot;
    private readonly IAutoSaveService    _autoSave;

    public AcquisitionController(
        IAcquisitionSession session,
        IExposureController exposure,
        ISnapshotCapture snapshot,
        IAutoSaveService autoSave)
    {
        _session  = session;
        _exposure = exposure;
        _snapshot = snapshot;
        _autoSave = autoSave;
    }

    [HttpPost("start")]
    public ActionResult Start([FromBody] StartAcquisitionRequest request)
    {
        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;
        _session.Start(profileId, triggerMode, request.FrameCount, request.IntervalMs);
        return Ok(new { message = "Acquisition started", profileId = profileId.Value });
    }

    [HttpPost("stop")]
    public ActionResult Stop()
    {
        _session.Stop();
        return Ok(new { message = "Acquisition stopped" });
    }

    [HttpDelete]
    public ActionResult ReleaseChannel()
    {
        _session.Stop();
        return Ok(new { message = "Channel released" });
    }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        var stats = _session.GetStatistics();
        return Ok(new
        {
            isActive     = _session.IsActive,
            channelState = _session.ChannelState.ToString().ToLowerInvariant(),
            profileId    = _session.ActiveProfileId?.Value,
            hasFrame     = _session.HasFrame,
            lastError    = _session.LastError,
            allowedActions = _session.GetAllowedActions(),
            statistics   = stats.HasValue ? new
            {
                frameCount              = stats.Value.FrameCount,
                droppedFrameCount       = stats.Value.DroppedFrameCount,
                errorCount              = stats.Value.ErrorCount,
                elapsedMs               = stats.Value.ElapsedTime.TotalMilliseconds,
                averageFps              = Math.Round(stats.Value.AverageFps, 2),
                minFrameIntervalMs      = Math.Round(stats.Value.MinFrameIntervalMs, 2),
                maxFrameIntervalMs      = Math.Round(stats.Value.MaxFrameIntervalMs, 2),
                averageFrameIntervalMs  = Math.Round(stats.Value.AverageFrameIntervalMs, 2),
                copyDropCount           = stats.Value.CopyDropCount,
                clusterUnavailableCount = stats.Value.ClusterUnavailableCount,
            } : null,
            recentEvents = _session.GetRecentEvents(50).Select(e => new
            {
                timestamp = e.Timestamp,
                type      = e.Type.ToString(),
                message   = e.Message,
            }),
        });
    }

    [HttpPost("trigger")]
    public async Task<ActionResult> Trigger()
    {
        var image   = await _session.TriggerAndWaitAsync(5000);
        var path    = await _autoSave.TrySaveAsync(image);
        if (path is not null)
            Response.Headers["X-Image-Path"] = path;

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;
        return File(stream, "image/png", "trigger.png");
    }

    [HttpGet("latest-frame")]
    public async Task<ActionResult> GetLatestFrame()
    {
        var frame = _session.GetLatestFrame();
        if (frame is null)
            return NoContent();

        var path = await _autoSave.TrySaveNewAsync(frame);
        if (path is not null)
            Response.Headers["X-Image-Path"] = path;

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(frame, stream);
        stream.Position = 0;
        return File(stream, "image/png", "latest.png");
    }

    [HttpGet("latest-frame/histogram")]
    public ActionResult GetHistogram()
    {
        var frame = _session.GetLatestFrame();
        if (frame is null) return NoContent();

        var histogram = HistogramService.Compute(frame);
        return Ok(new { red = histogram.Red, green = histogram.Green, blue = histogram.Blue, bins = 256 });
    }

    [HttpPost("snapshot")]
    public async Task<ActionResult> Snapshot([FromBody] SnapshotRequest request)
    {
        if (_session.IsActive)
            throw new AcquisitionConflictException("Cannot snapshot while acquisition is active.");

        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;

        string filePath;
        if (!string.IsNullOrWhiteSpace(request.OutputPath))
        {
            // Custom output path: bypass DB recording
            var image = await CaptureRawAsync(profileId, triggerMode);
            new ImageWriter().Save(image, request.OutputPath);
            filePath = request.OutputPath;
        }
        else
        {
            filePath = await _snapshot.CaptureAsync(profileId, triggerMode);
        }

        Response.Headers["X-Image-Path"] = filePath;

        // Re-read the saved file to return PNG preview
        var savedImage = LoadImageFromFile(filePath);
        var encoder    = new PngEncoder();
        var stream     = new MemoryStream();
        encoder.Encode(savedImage, stream);
        stream.Position = 0;
        return File(stream, "image/png", "snapshot.png");
    }

    [HttpGet("exposure")]
    public ActionResult GetExposure() => Ok(_exposure.GetExposure());

    [HttpPut("exposure")]
    public ActionResult SetExposure([FromBody] SetExposureRequest request)
        => Ok(_exposure.SetExposure(request.ExposureUs));

    // --- helpers ---

    private async Task<ImageData> CaptureRawAsync(ProfileId profileId, TriggerMode? triggerMode)
    {
        _session.Start(profileId, triggerMode, frameCount: 1);
        try { return await _session.TriggerAndWaitAsync(5000); }
        finally { _session.Stop(); }
    }

    private static ImageData LoadImageFromFile(string filePath)
    {
        using var img = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(filePath);
        var pixels = new byte[img.Width * img.Height * 3];
        img.CopyPixelDataTo(pixels);
        return new ImageData(pixels, img.Width, img.Height, img.Width * 3);
    }
}

public class StartAcquisitionRequest
{
    public required string ProfileId  { get; set; }
    public string? TriggerMode        { get; set; }
    public int?    FrameCount         { get; set; }
    public int?    IntervalMs         { get; set; }
}

public class SnapshotRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode       { get; set; }
    public string? OutputPath        { get; set; }
}

public class SetExposureRequest
{
    public double? ExposureUs { get; set; }
}
