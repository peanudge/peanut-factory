using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Exceptions;
using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/cameras")]
public class CameraController : ControllerBase
{
    private readonly CameraRegistry  _registry;
    private readonly ISnapshotCapture _snapshot;
    private readonly IAutoSaveService _autoSave;

    public CameraController(
        CameraRegistry registry,
        ISnapshotCapture snapshot,
        IAutoSaveService autoSave)
    {
        _registry = registry;
        _snapshot = snapshot;
        _autoSave = autoSave;
    }

    // GET /api/cameras
    [HttpGet]
    public ActionResult ListCameras()
        => Ok(new { cameras = _registry.GetAllIds().Select(id => new { id }) });

    // POST /api/cameras/{cameraId}/start
    [HttpPost("{cameraId}/start")]
    public async Task<ActionResult> Start(string cameraId, [FromBody] StartAcquisitionRequest request)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;
        await actor.StartAsync(profileId, triggerMode, request.FrameCount, request.IntervalMs);
        return Ok(new { message = "Acquisition started", profileId = profileId.Value });
    }

    // POST /api/cameras/{cameraId}/stop
    [HttpPost("{cameraId}/stop")]
    public async Task<ActionResult> Stop(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });
        await actor.StopAsync();
        return Ok(new { message = "Acquisition stopped" });
    }

    // GET /api/cameras/{cameraId}/status
    [HttpGet("{cameraId}/status")]
    public async Task<ActionResult> GetStatus(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var s     = await actor.GetStatusAsync();
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

    // POST /api/cameras/{cameraId}/trigger
    [HttpPost("{cameraId}/trigger")]
    public async Task<ActionResult> Trigger(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var image = await actor.TriggerAsync(5000, HttpContext.RequestAborted);
        var path  = await _autoSave.TrySaveAsync(image);
        if (path is not null)
            Response.Headers["X-Image-Path"] = path;

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;
        return File(stream, "image/png", "trigger.png");
    }

    // GET /api/cameras/{cameraId}/latest-frame
    [HttpGet("{cameraId}/latest-frame")]
    public async Task<ActionResult> GetLatestFrame(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var result = await actor.GetLatestFrameAsync();
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

    // GET /api/cameras/{cameraId}/latest-frame/histogram
    [HttpGet("{cameraId}/latest-frame/histogram")]
    public async Task<ActionResult> GetHistogram(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var result = await actor.GetLatestFrameAsync();
        if (result.Frame is null) return NoContent();

        var histogram = HistogramService.Compute(result.Frame);
        return Ok(new { red = histogram.Red, green = histogram.Green, blue = histogram.Blue, bins = 256 });
    }

    // POST /api/cameras/{cameraId}/snapshot
    [HttpPost("{cameraId}/snapshot")]
    public async Task<ActionResult> Snapshot(string cameraId, [FromBody] SnapshotRequest request)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var status = await actor.GetStatusAsync();
        if (status.IsActive)
            throw new AcquisitionConflictException("Cannot snapshot while acquisition is active.");

        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;

        string filePath;
        if (!string.IsNullOrWhiteSpace(request.OutputPath))
        {
            await actor.StartAsync(profileId, triggerMode, frameCount: 1);
            PeanutVision.MultiCamDriver.Imaging.ImageData rawImage;
            try { rawImage = await actor.TriggerAsync(5000, HttpContext.RequestAborted); }
            finally { await actor.StopAsync(); }
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

    // GET /api/cameras/{cameraId}/exposure
    [HttpGet("{cameraId}/exposure")]
    public async Task<ActionResult> GetExposure(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });
        return Ok(await actor.GetExposureAsync());
    }

    // PUT /api/cameras/{cameraId}/exposure
    [HttpPut("{cameraId}/exposure")]
    public async Task<ActionResult> SetExposure(string cameraId, [FromBody] SetExposureRequest request)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });
        return Ok(await actor.SetExposureAsync(request.ExposureUs));
    }

    private ICameraActor? GetActorOrNotFound(string cameraId)
        => _registry.TryGet(cameraId);

    private static PeanutVision.MultiCamDriver.Imaging.ImageData LoadImageFromFile(string filePath)
    {
        using var img = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(filePath);
        var pixels = new byte[img.Width * img.Height * 3];
        img.CopyPixelDataTo(pixels);
        return new PeanutVision.MultiCamDriver.Imaging.ImageData(pixels, img.Width, img.Height, img.Width * 3);
    }
}
