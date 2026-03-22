using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LatencyController : ControllerBase
{
    private readonly ILatencyService _latency;

    public LatencyController(ILatencyService latency)
        => _latency = latency;

    /// <summary>Returns the most recent latency records (default: 200, max: 1000).</summary>
    [HttpGet("records")]
    public ActionResult GetRecords([FromQuery] int limit = 200)
    {
        var records = _latency.GetRecent(Math.Clamp(limit, 1, 1000));
        return Ok(records.Select(r => new
        {
            id              = r.Id,
            triggerSentAt   = r.TriggerSentAt,
            frameReceivedAt = r.FrameReceivedAt,
            latencyMs       = r.LatencyMs,
            frameIndex      = r.FrameIndex,
            profileId       = r.ProfileId,
        }));
    }

    /// <summary>Returns aggregate statistics (min, max, mean, p50, p95, p99, stddev) over all stored records.</summary>
    [HttpGet("stats")]
    public ActionResult GetStats()
    {
        var stats = _latency.GetStats();
        if (stats is null)
            return NoContent();

        return Ok(new
        {
            count    = stats.Count,
            minMs    = stats.MinMs,
            maxMs    = stats.MaxMs,
            meanMs   = stats.MeanMs,
            p50Ms    = stats.P50Ms,
            p95Ms    = stats.P95Ms,
            p99Ms    = stats.P99Ms,
            stdDevMs = stats.StdDevMs,
        });
    }

    /// <summary>Clears all stored latency records.</summary>
    [HttpDelete("records")]
    public ActionResult Clear()
    {
        _latency.Clear();
        return Ok(new { message = "Latency records cleared" });
    }
}
