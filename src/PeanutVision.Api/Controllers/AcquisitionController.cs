using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Exceptions;
using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcquisitionController : ControllerBase
{
    // The legacy /api/acquisition/... endpoints delegate to the "cam-1" actor.
    // This controller is intentionally temporary — it will be deleted in Stage 4.
    private const string DefaultCameraId = "cam-1";

    private readonly CameraRegistry  _registry;
    private readonly ISnapshotCapture _snapshot;
    private readonly IAutoSaveService _autoSave;

    public AcquisitionController(
        CameraRegistry registry,
        ISnapshotCapture snapshot,
        IAutoSaveService autoSave)
    {
        _registry = registry;
        _snapshot = snapshot;
        _autoSave = autoSave;
    }

    private ICameraActor DefaultActor =>
        _registry.TryGet(DefaultCameraId)
        ?? throw new InvalidOperationException($"Default camera '{DefaultCameraId}' is not registered.");

    [HttpPost("start")]
    public async Task<ActionResult> Start([FromBody] StartAcquisitionRequest request)
    {
        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;
        await DefaultActor.StartAsync(profileId, triggerMode, request.FrameCount, request.IntervalMs);
        return Ok(new { message = "Acquisition started", profileId = profileId.Value });
    }

    [HttpPost("stop")]
    public async Task<ActionResult> Stop()
    {
        await DefaultActor.StopAsync();
        return Ok(new { message = "Acquisition stopped" });
    }

    [HttpDelete]
    public async Task<ActionResult> ReleaseChannel()
    {
        await DefaultActor.StopAsync();
        return Ok(new { message = "Channel released" });
    }

    [HttpGet("status")]
    public async Task<ActionResult> GetStatus()
    {
        var s     = await DefaultActor.GetStatusAsync();
        var stats = s.Statistics;
        return Ok(new
        {
            isActive       = s.IsActive,
            channelState   = s.ChannelState.ToString().ToLowerInvariant(),
            profileId      = s.ActiveProfileId,
            hasFrame       = s.HasFrame,
            lastError      = s.LastError,
            allowedActions = s.AllowedActions,
            statistics     = stats.HasValue ? new
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
            } : (object?)null,
            recentEvents = s.RecentEvents.Select(e => new
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
        var image = await DefaultActor.TriggerAsync(5000, HttpContext.RequestAborted);
        var path  = await _autoSave.TrySaveAsync(image);
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
        var result = await DefaultActor.GetLatestFrameAsync();
        if (result.Frame is null) return NoContent();

        if (result.IsNew)
        {
            var path = await _autoSave.TrySaveAsync(result.Frame);
            if (path is not null)
                Response.Headers["X-Image-Path"] = path;
        }

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(result.Frame, stream);
        stream.Position = 0;
        return File(stream, "image/png", "latest.png");
    }

    [HttpGet("latest-frame/histogram")]
    public async Task<ActionResult> GetHistogram()
    {
        var result = await DefaultActor.GetLatestFrameAsync();
        if (result.Frame is null) return NoContent();

        var histogram = HistogramService.Compute(result.Frame);
        return Ok(new { red = histogram.Red, green = histogram.Green, blue = histogram.Blue, bins = 256 });
    }

    [HttpPost("snapshot")]
    public async Task<ActionResult> Snapshot([FromBody] SnapshotRequest request)
    {
        var status = await DefaultActor.GetStatusAsync();
        if (status.IsActive)
            throw new AcquisitionConflictException("Cannot snapshot while acquisition is active.");

        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;

        string filePath;
        if (!string.IsNullOrWhiteSpace(request.OutputPath))
        {
            await DefaultActor.StartAsync(profileId, triggerMode, frameCount: 1);
            PeanutVision.MultiCamDriver.Imaging.ImageData rawImage;
            try { rawImage = await DefaultActor.TriggerAsync(5000, HttpContext.RequestAborted); }
            finally { await DefaultActor.StopAsync(); }
            new PeanutVision.MultiCamDriver.Imaging.ImageWriter().Save(rawImage, request.OutputPath);
            filePath = request.OutputPath;
        }
        else
        {
            filePath = await _snapshot.CaptureAsync(profileId, triggerMode);
        }

        Response.Headers["X-Image-Path"] = filePath;

        var savedImage = LoadImageFromFile(filePath);
        var encoder    = new PngEncoder();
        var stream     = new MemoryStream();
        encoder.Encode(savedImage, stream);
        stream.Position = 0;
        return File(stream, "image/png", "snapshot.png");
    }

    [HttpGet("exposure")]
    public async Task<ActionResult> GetExposure()
        => Ok(await DefaultActor.GetExposureAsync());

    [HttpPut("exposure")]
    public async Task<ActionResult> SetExposure([FromBody] SetExposureRequest request)
        => Ok(await DefaultActor.SetExposureAsync(request.ExposureUs));

    private static PeanutVision.MultiCamDriver.Imaging.ImageData LoadImageFromFile(string filePath)
    {
        using var img = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(filePath);
        var pixels = new byte[img.Width * img.Height * 3];
        img.CopyPixelDataTo(pixels);
        return new PeanutVision.MultiCamDriver.Imaging.ImageData(pixels, img.Width, img.Height, img.Width * 3);
    }
}

// Request DTOs remain here (same as before)
public class StartAcquisitionRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode       { get; set; }
    public int?    FrameCount        { get; set; }
    public int?    IntervalMs        { get; set; }
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
