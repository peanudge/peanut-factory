using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver.Imaging;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcquisitionController : ControllerBase
{
    private readonly IAcquisitionService _acquisition;

    public AcquisitionController(IAcquisitionService acquisition)
    {
        _acquisition = acquisition;
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

            _acquisition.Start(profileId, triggerMode);
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
                }
                : null,
        });
    }

    [HttpPost("trigger")]
    public ActionResult Trigger()
    {
        try
        {
            _acquisition.SendTrigger();
            return Ok(new { message = "Software trigger sent" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("capture")]
    public ActionResult Capture()
    {
        var frame = _acquisition.CaptureFrame();
        if (frame == null)
            return NotFound(new { error = "No frame available. Start acquisition first." });

        var encoder = new PngEncoder();
        var stream = new MemoryStream();
        encoder.Encode(frame, stream);
        stream.Position = 0;

        return File(stream, "image/png", "capture.png");
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
                var writer = new ImageWriter();
                writer.Save(image, request.OutputPath);
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
}

public class SnapshotRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode { get; set; }
    public string? OutputPath { get; set; }
}
