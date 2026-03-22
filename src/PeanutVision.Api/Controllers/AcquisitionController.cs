using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcquisitionController : ControllerBase
{
    private readonly IAcquisitionService _acquisition;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly FilenameGenerator _filenameGenerator;
    private readonly FrameSaveTracker _frameSaveTracker;
    private readonly ICapturedImageRepository _imageRepository;
    private readonly IThumbnailService _thumbnailService;
    private readonly ISessionRepository _sessionRepository;
    private readonly string _contentRootPath;

    public AcquisitionController(
        IAcquisitionService acquisition,
        IImageSaveSettingsService saveSettings,
        FilenameGenerator filenameGenerator,
        FrameSaveTracker frameSaveTracker,
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService,
        ISessionRepository sessionRepository,
        IWebHostEnvironment environment)
    {
        _acquisition = acquisition;
        _saveSettings = saveSettings;
        _filenameGenerator = filenameGenerator;
        _frameSaveTracker = frameSaveTracker;
        _imageRepository = imageRepository;
        _thumbnailService = thumbnailService;
        _sessionRepository = sessionRepository;
        _contentRootPath = environment.ContentRootPath;
    }

    [HttpPost("start")]
    public ActionResult Start([FromBody] StartAcquisitionRequest request)
    {
        var profileId = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null
            ? TriggerMode.Parse(request.TriggerMode)
            : (TriggerMode?)null;

        // Auto-release any idle channel so callers don't need explicit channel management
        if (_acquisition.ChannelState != ChannelState.None && _acquisition.ChannelState != ChannelState.Active)
            _acquisition.ReleaseChannel();

        _acquisition.CreateChannel(profileId, triggerMode);
        _acquisition.Start(request.FrameCount, request.IntervalMs);
        return Ok(new { message = "Acquisition started", profileId = profileId.Value });
    }

    [HttpPost("stop")]
    public ActionResult Stop()
    {
        _acquisition.Stop();
        return Ok(new { message = "Acquisition stopped" });
    }

    [HttpDelete]
    public ActionResult ReleaseChannel()
    {
        _acquisition.ReleaseChannel();
        return Ok(new { message = "Channel released" });
    }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        var stats = _acquisition.GetStatistics();
        return Ok(new
        {
            isActive = _acquisition.IsActive,
            channelState = _acquisition.ChannelState.ToString().ToLowerInvariant(),
            profileId = _acquisition.ActiveProfileId?.Value,
            hasFrame = _acquisition.HasFrame,
            lastError = _acquisition.LastError,
            allowedActions = _acquisition.GetAllowedActions(),
            statistics = stats.HasValue
                ? new
                {
                    frameCount = stats.Value.FrameCount,
                    droppedFrameCount = stats.Value.DroppedFrameCount,
                    errorCount = stats.Value.ErrorCount,
                    elapsedMs = stats.Value.ElapsedTime.TotalMilliseconds,
                    averageFps = Math.Round(stats.Value.AverageFps, 2),
                    minFrameIntervalMs = Math.Round(stats.Value.MinFrameIntervalMs, 2),
                    maxFrameIntervalMs = Math.Round(stats.Value.MaxFrameIntervalMs, 2),
                    averageFrameIntervalMs = Math.Round(stats.Value.AverageFrameIntervalMs, 2),
                    copyDropCount = stats.Value.CopyDropCount,
                    clusterUnavailableCount = stats.Value.ClusterUnavailableCount,
                }
                : null,
            recentEvents = _acquisition.GetRecentEvents(50).Select(e => new
            {
                timestamp = e.Timestamp,
                type = e.Type.ToString(),
                message = e.Message,
            }),
        });
    }

    [HttpPost("trigger")]
    public async Task<ActionResult> Trigger()
    {
        var image = await _acquisition.TriggerAndWaitAsync(5000);

        var settings = _saveSettings.GetSettings();
        if (settings.AutoSave)
        {
            var filePath = await SaveAndRecordAsync(
                image, settings, _acquisition.ActiveProfileId?.Value);
            Response.Headers["X-Image-Path"] = filePath;
        }

        var encoder = new PngEncoder();
        var stream = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;

        return File(stream, "image/png", "trigger.png");
    }

    [HttpGet("latest-frame")]
    public async Task<ActionResult> GetLatestFrame()
    {
        var frame = _acquisition.GetLatestFrame();
        if (frame is null)
            return NoContent();

        var settings = _saveSettings.GetSettings();
        if (settings.AutoSave && _frameSaveTracker.ShouldSave(frame))
        {
            var filePath = await SaveAndRecordAsync(
                frame, settings, _acquisition.ActiveProfileId?.Value);
            Response.Headers["X-Image-Path"] = filePath;
        }

        var encoder = new PngEncoder();
        var stream = new MemoryStream();
        encoder.Encode(frame, stream);
        stream.Position = 0;

        return File(stream, "image/png", "latest.png");
    }

    [HttpGet("latest-frame/histogram")]
    public ActionResult GetHistogram()
    {
        var frame = _acquisition.GetLatestFrame();
        if (frame is null)
            return NoContent();

        var histogram = HistogramService.Compute(frame);
        return Ok(new
        {
            red = histogram.Red,
            green = histogram.Green,
            blue = histogram.Blue,
            bins = 256,
        });
    }

    [HttpPost("snapshot")]
    public async Task<ActionResult> Snapshot([FromBody] SnapshotRequest request)
    {
        var profileId = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null
            ? TriggerMode.Parse(request.TriggerMode)
            : (TriggerMode?)null;

        var image = _acquisition.Snapshot(profileId, triggerMode);

        if (!string.IsNullOrWhiteSpace(request.OutputPath))
        {
            new ImageWriter().Save(image, request.OutputPath);
            Response.Headers["X-Image-Path"] = request.OutputPath;
        }
        else
        {
            var settings = _saveSettings.GetSettings();
            if (settings.AutoSave)
            {
                var filePath = await SaveAndRecordAsync(image, settings, request.ProfileId);
                Response.Headers["X-Image-Path"] = filePath;
            }
        }

        var encoder = new PngEncoder();
        var stream = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;

        return File(stream, "image/png", "snapshot.png");
    }

    private async Task<string> SaveAndRecordAsync(
        ImageData image, ImageSaveSettings settings, string? profileId)
    {
        var filePath = _filenameGenerator.Generate(settings, _contentRootPath, profileId);
        new ImageWriter().Save(image, filePath);

        var thumbPath = await _thumbnailService.GenerateAsync(filePath);
        var activeSession = await _sessionRepository.GetActiveAsync();
        var fileInfo = new FileInfo(filePath);

        await _imageRepository.AddAsync(new CapturedImage
        {
            Id = Guid.NewGuid(),
            FilePath = filePath,
            ThumbnailPath = thumbPath,
            Width = image.Width,
            Height = image.Height,
            FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
            Format = settings.Format.ToString().ToLower(),
            CapturedAt = DateTime.UtcNow,
            SessionId = activeSession?.Id,
        });

        return filePath;
    }
}

public class StartAcquisitionRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode { get; set; }
    public int? FrameCount { get; set; }
    public int? IntervalMs { get; set; }
}

public class SnapshotRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode { get; set; }
    public string? OutputPath { get; set; }
}
