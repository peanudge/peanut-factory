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
    private readonly string _contentRootPath;

    public AcquisitionController(
        IAcquisitionService acquisition,
        IImageSaveSettingsService saveSettings,
        FilenameGenerator filenameGenerator,
        FrameSaveTracker frameSaveTracker,
        IWebHostEnvironment environment)
    {
        _acquisition = acquisition;
        _saveSettings = saveSettings;
        _filenameGenerator = filenameGenerator;
        _frameSaveTracker = frameSaveTracker;
        _contentRootPath = environment.ContentRootPath;
    }

    [HttpPost("start")]
    public ActionResult Start([FromBody] StartAcquisitionRequest request)
    {
        try
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (MultiCamException ex)
        {
            return StatusCode(502, new { error = ErrorMessageSanitizer.Sanitize(ex) });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
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
        try
        {
            _acquisition.ReleaseChannel();
            return Ok(new { message = "Channel released" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        var stats = _acquisition.GetStatistics();
        return Ok(new
        {
            isActive = _acquisition.IsActive,
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
        try
        {
            var image = await _acquisition.TriggerAndWaitAsync(5000);

            var settings = _saveSettings.GetSettings();
            if (settings.AutoSave)
            {
                var filePath = _filenameGenerator.Generate(
                    settings, _contentRootPath, _acquisition.ActiveProfileId?.Value);
                new ImageWriter().Save(image, filePath);
                Response.Headers["X-Image-Path"] = filePath;
            }

            var encoder = new PngEncoder();
            var stream = new MemoryStream();
            encoder.Encode(image, stream);
            stream.Position = 0;

            return File(stream, "image/png", "trigger.png");
        }
        catch (MultiCamException ex)
        {
            return StatusCode(502, new { error = ErrorMessageSanitizer.Sanitize(ex) });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (TimeoutException ex)
        {
            return StatusCode(504, new { error = ex.Message });
        }
    }

    [HttpGet("latest-frame")]
    public ActionResult GetLatestFrame()
    {
        var frame = _acquisition.GetLatestFrame();
        if (frame is null)
            return NoContent();

        var settings = _saveSettings.GetSettings();
        if (settings.AutoSave && _frameSaveTracker.ShouldSave(frame))
        {
            var filePath = _filenameGenerator.Generate(
                settings, _contentRootPath, _acquisition.ActiveProfileId?.Value);
            new ImageWriter().Save(frame, filePath);
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
    public ActionResult Snapshot([FromBody] SnapshotRequest request)
    {
        try
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
                    var filePath = _filenameGenerator.Generate(
                        settings, _contentRootPath, request.ProfileId);
                    new ImageWriter().Save(image, filePath);
                    Response.Headers["X-Image-Path"] = filePath;
                }
            }

            var encoder = new PngEncoder();
            var stream = new MemoryStream();
            encoder.Encode(image, stream);
            stream.Position = 0;

            return File(stream, "image/png", "snapshot.png");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (MultiCamException ex)
        {
            return StatusCode(502, new { error = ErrorMessageSanitizer.Sanitize(ex) });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (TimeoutException ex)
        {
            return StatusCode(504, new { error = ex.Message });
        }
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
