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
    private readonly IAcquisitionSession _acquisition;
    private readonly AcquisitionConfigValidator _validator;
    private readonly IHostApplicationLifetime _appLifetime;

    public AcquisitionController(
        IAcquisitionSession acquisition,
        AcquisitionConfigValidator validator,
        IHostApplicationLifetime appLifetime)
    {
        _acquisition = acquisition;
        _validator = validator;
        _appLifetime = appLifetime;
    }

    [HttpPost("start")]
    public ActionResult Start([FromBody] StartAcquisitionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProfileId))
            return BadRequest(new { error = "profileId is required." });

        if (request.Format is not null && !Enum.TryParse<SaveImageFormat>(request.Format, ignoreCase: true, out _))
            return BadRequest(new { error = $"Invalid format '{request.Format}'. Valid values: png, bmp, raw." });

        var format = request.Format is not null && Enum.TryParse<SaveImageFormat>(request.Format, ignoreCase: true, out var f)
            ? f : SaveImageFormat.Png;

        var config = new AcquisitionConfig(
            new ProfileId(request.ProfileId),
            request.FrameCount,
            request.IntervalMs,
            request.OutputDirectory ?? "CapturedImages",
            format);

        var validation = _validator.Validate(config);
        if (!validation.IsValid)
            return BadRequest(new
            {
                error = "Invalid acquisition configuration.",
                errors = validation.Errors.Select(e => new { field = e.Field, message = e.Message }),
            });

        if (_acquisition.GetStatus().ChannelState == ChannelState.Active)
            return Conflict(new { error = "Acquisition is already running. Stop it first." });

        _acquisition.Start(config);
        return Ok(new { message = "Acquisition started", profileId = config.ProfileId.Value });
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
        var s = _acquisition.GetStatus();
        return Ok(new
        {
            isActive = s.IsActive,
            channelState = s.ChannelState.ToString().ToLowerInvariant(),
            profileId = s.ActiveConfig?.ProfileId.Value,
            activeFrameCount = s.IsActive ? s.ActiveConfig?.FrameCount : null,
            activeIntervalMs = s.IsActive ? s.ActiveConfig?.IntervalMs : null,
            outputDirectory = s.ActiveConfig?.OutputDirectory,
            format = s.ActiveConfig?.Format.ToString().ToLower(),
            hasFrame = s.HasFrame,
            lastError = s.LastError,
            allowedActions = s.AllowedActions,
            statistics = s.Statistics.HasValue
                ? new
                {
                    frameCount = s.Statistics.Value.FrameCount,
                    droppedFrameCount = s.Statistics.Value.DroppedFrameCount,
                    errorCount = s.Statistics.Value.ErrorCount,
                    elapsedMs = s.Statistics.Value.ElapsedTime.TotalMilliseconds,
                    averageFps = Math.Round(s.Statistics.Value.AverageFps, 2),
                    minFrameIntervalMs = Math.Round(s.Statistics.Value.MinFrameIntervalMs, 2),
                    maxFrameIntervalMs = Math.Round(s.Statistics.Value.MaxFrameIntervalMs, 2),
                    averageFrameIntervalMs = Math.Round(s.Statistics.Value.AverageFrameIntervalMs, 2),
                    copyDropCount = s.Statistics.Value.CopyDropCount,
                    clusterUnavailableCount = s.Statistics.Value.ClusterUnavailableCount,
                }
                : null,
            recentEvents = s.RecentEvents.Select(e => new
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

        // Link request abort with host shutdown so the SSE loop exits immediately on app stop
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, _appLifetime.ApplicationStopping);
        var token = cts.Token;

        var channel = Channel.CreateUnbounded<string>(
            new UnboundedChannelOptions { SingleReader = true });

        void OnFrameAcquired(object? _, EventArgs __) =>
            channel.Writer.TryWrite($"event: frame_ready\ndata: {{\"timestamp\":\"{DateTimeOffset.UtcNow:O}\"}}\n\n");

        void OnStatusChanged(object? _, EventArgs __) =>
            channel.Writer.TryWrite($"event: status_changed\ndata: {BuildStatusJson()}\n\n");

        _acquisition.FrameAcquired += OnFrameAcquired;
        _acquisition.StatusChanged += OnStatusChanged;

        channel.Writer.TryWrite($"event: status_changed\ndata: {BuildStatusJson()}\n\n");

        try
        {
            await foreach (var text in channel.Reader.ReadAllAsync(token))
            {
                await Response.WriteAsync(text, token);
                await Response.Body.FlushAsync(token);
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
        var image = await _acquisition.TriggerAsync(5000);
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
        return Ok(new { red = histogram.Red, green = histogram.Green, blue = histogram.Blue, bins = 256 });
    }

    private string BuildStatusJson()
    {
        var s = _acquisition.GetStatus();
        var payload = new
        {
            isActive = s.IsActive,
            channelState = s.ChannelState.ToString().ToLowerInvariant(),
            profileId = s.ActiveConfig?.ProfileId.Value,
            activeFrameCount = s.IsActive ? s.ActiveConfig?.FrameCount : (int?)null,
            activeIntervalMs = s.IsActive ? s.ActiveConfig?.IntervalMs : (int?)null,
            outputDirectory = s.ActiveConfig?.OutputDirectory,
            format = s.ActiveConfig?.Format.ToString().ToLower(),
            hasFrame = s.HasFrame,
            lastError = s.LastError,
            allowedActions = s.AllowedActions.Select(a => a.ToString().ToLowerInvariant()).ToArray(),
            statistics = s.Statistics.HasValue
                ? (object)new
                {
                    frameCount = s.Statistics.Value.FrameCount,
                    droppedFrameCount = s.Statistics.Value.DroppedFrameCount,
                    errorCount = s.Statistics.Value.ErrorCount,
                    elapsedMs = s.Statistics.Value.ElapsedTime.TotalMilliseconds,
                    averageFps = Math.Round(s.Statistics.Value.AverageFps, 2),
                    minFrameIntervalMs = Math.Round(s.Statistics.Value.MinFrameIntervalMs, 2),
                    maxFrameIntervalMs = Math.Round(s.Statistics.Value.MaxFrameIntervalMs, 2),
                    averageFrameIntervalMs = Math.Round(s.Statistics.Value.AverageFrameIntervalMs, 2),
                    copyDropCount = s.Statistics.Value.CopyDropCount,
                    clusterUnavailableCount = s.Statistics.Value.ClusterUnavailableCount,
                }
                : null,
            recentEvents = s.RecentEvents.Select(e => new
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
    public int? FrameCount { get; set; }
    public int? IntervalMs { get; set; }
    public string? OutputDirectory { get; set; }
    public string? Format { get; set; }
}
