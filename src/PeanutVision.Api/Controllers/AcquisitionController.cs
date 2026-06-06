using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver.Imaging.Encoders;
using System.Text.Json;
using System.Threading.Channels;

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
        if (_acquisition.ChannelState == ChannelState.Active)
            return Conflict(new { error = "Acquisition is already running. Stop it first." });

        var profileId = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null
            ? TriggerMode.Parse(request.TriggerMode)
            : (TriggerMode?)null;

        if (_acquisition.ChannelState == ChannelState.Idle)
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
            triggerMode = _acquisition.ChannelTriggerMode?.ToString().ToLowerInvariant(),
            activeFrameCount = _acquisition.ActiveFrameCount,
            activeIntervalMs = _acquisition.ActiveIntervalMs,
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

    [HttpGet("events")]
    public async Task GetEvents(CancellationToken ct)
    {
        Response.ContentType = "text/event-stream; charset=utf-8";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        var channel = Channel.CreateUnbounded<string>(
            new UnboundedChannelOptions { SingleReader = true });

        void OnFrameAcquired(object? _, EventArgs __) =>
            channel.Writer.TryWrite($"event: frame_ready\ndata: {{\"timestamp\":\"{DateTimeOffset.UtcNow:O}\"}}\n\n");

        void OnStatusChanged(object? _, EventArgs __) =>
            channel.Writer.TryWrite($"event: status_changed\ndata: {BuildStatusJson()}\n\n");

        _acquisition.FrameAcquired += OnFrameAcquired;
        _acquisition.StatusChanged += OnStatusChanged;

        // Send current status immediately so client has initial state
        channel.Writer.TryWrite($"event: status_changed\ndata: {BuildStatusJson()}\n\n");

        try
        {
            await foreach (var text in channel.Reader.ReadAllAsync(ct))
            {
                await Response.WriteAsync(text, ct);
                await Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _acquisition.FrameAcquired -= OnFrameAcquired;
            _acquisition.StatusChanged -= OnStatusChanged;
            channel.Writer.TryComplete();
        }
    }

    [HttpPost("trigger")]
    public async Task<ActionResult> Trigger()
    {
        var image = await _acquisition.TriggerAndWaitAsync(5000);

        var encoder = new PngEncoder();
        var stream = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;

        return File(stream, "image/png", "trigger.png");
    }

    [HttpGet("latest-frame")]
    public ActionResult GetLatestFrame()
    {
        var frame = _acquisition.GetLatestFrame();
        if (frame is null)
            return NoContent();

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

    private string BuildStatusJson()
    {
        var stats = _acquisition.GetStatistics();
        var payload = new
        {
            isActive = _acquisition.IsActive,
            channelState = _acquisition.ChannelState.ToString().ToLowerInvariant(),
            profileId = _acquisition.ActiveProfileId?.Value,
            triggerMode = _acquisition.ChannelTriggerMode?.ToString().ToLowerInvariant(),
            activeFrameCount = _acquisition.ActiveFrameCount,
            activeIntervalMs = _acquisition.ActiveIntervalMs,
            hasFrame = _acquisition.HasFrame,
            lastError = _acquisition.LastError,
            allowedActions = _acquisition.GetAllowedActions()
                .Select(a => a.ToString().ToLowerInvariant()).ToArray(),
            statistics = stats.HasValue
                ? (object)new
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
        };
        return JsonSerializer.Serialize(payload);
    }

}

public class StartAcquisitionRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode { get; set; }
    public int? FrameCount { get; set; }
    public int? IntervalMs { get; set; }
}
