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
    private readonly AcquisitionManager _manager;

    public AcquisitionController(AcquisitionManager manager)
    {
        _manager = manager;
    }

    [HttpPost("start")]
    public ActionResult Start([FromBody] StartAcquisitionRequest request)
    {
        try
        {
            McTrigMode? triggerMode = request.TriggerMode != null
                ? Enum.Parse<McTrigMode>(request.TriggerMode, ignoreCase: true)
                : null;

            _manager.Start(request.ProfileId, triggerMode);
            return Ok(new { message = "Acquisition started", profileId = request.ProfileId });
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
        _manager.Stop();
        return Ok(new { message = "Acquisition stopped" });
    }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        var stats = _manager.GetStatistics();
        return Ok(new
        {
            isActive = _manager.IsActive,
            profileId = _manager.ActiveProfileId,
            hasFrame = _manager.LastFrame != null,
            lastError = _manager.LastError,
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
            _manager.SendTrigger();
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
        var frame = _manager.CaptureFrame();
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
            McTrigMode? triggerMode = request.TriggerMode != null
                ? Enum.Parse<McTrigMode>(request.TriggerMode, ignoreCase: true)
                : null;

            var image = _manager.Snapshot(request.ProfileId, triggerMode);

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
