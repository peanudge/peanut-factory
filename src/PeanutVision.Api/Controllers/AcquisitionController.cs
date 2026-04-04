using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;
using PeanutVision.Capture;
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
    private readonly IFrameWriter _frameWriter;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _contentRootPath;

    /// <summary>Tracks which frame reference was last saved via latest-frame to avoid duplicate saves on repeated polls.</summary>
    private static ImageData? _lastLatestFrameSaved;
    private static readonly object _saveTrackLock = new();

    public AcquisitionController(
        IAcquisitionSession session,
        IExposureController exposure,
        ISnapshotCapture snapshot,
        IFrameWriter frameWriter,
        IImageSaveSettingsService saveSettings,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment environment)
    {
        _session  = session;
        _exposure = exposure;
        _snapshot = snapshot;
        _frameWriter = frameWriter;
        _saveSettings = saveSettings;
        _scopeFactory = scopeFactory;
        _contentRootPath = environment.ContentRootPath;
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
        var image = await _session.TriggerAndWaitAsync(5000);

        await SaveFrameIfAutoSaveEnabledAsync(image);

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

        // Only save on the first poll of a new frame to avoid duplicate writes
        bool isNewFrame;
        lock (_saveTrackLock)
        {
            isNewFrame = !ReferenceEquals(frame, _lastLatestFrameSaved);
            if (isNewFrame) _lastLatestFrameSaved = frame;
        }

        if (isNewFrame)
            await SaveFrameIfAutoSaveEnabledAsync(frame);

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
            throw new Exceptions.AcquisitionConflictException(
                "Cannot take a snapshot while acquisition is active. Stop it first.");

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

    /// <summary>
    /// Saves the frame to disk and records it in the DB when autosave is enabled.
    /// Sets the X-Image-Path response header on success.
    /// </summary>
    private async Task SaveFrameIfAutoSaveEnabledAsync(ImageData frame)
    {
        var settings = _saveSettings.GetSettings();
        if (!settings.AutoSave) return;

        var options = settings.ToWriterOptions(_contentRootPath);
        var filePath = _frameWriter.Write(frame, options);

        Response.Headers["X-Image-Path"] = filePath;

        var fileInfo = new FileInfo(filePath);
        var savedEvent = new FrameSavedEvent(
            FilePath: filePath,
            CapturedAt: DateTimeOffset.UtcNow,
            Width: frame.Width,
            Height: frame.Height,
            FileSizeBytes: fileInfo.Exists ? fileInfo.Length : 0,
            Format: settings.Format.ToString().ToLower());

        await using var scope = _scopeFactory.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<FrameSavedHandler>();
        await handler.HandleAsync(savedEvent);
    }

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
