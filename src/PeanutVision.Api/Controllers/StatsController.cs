using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Controllers;

/// <summary>
/// 시간대별 이미지 취득 건수 통계를 조회하는 API.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly ICaptureStatService _captureStatService;

    public StatsController(ICaptureStatService captureStatService)
        => _captureStatService = captureStatService;

    /// <summary>
    /// 시간대별 취득 건수 통계를 반환한다.
    ///
    /// <para>
    /// <b>모드 1 — 최근 N시간:</b> <c>hours</c> 파라미터만 지정 (기본값 24).
    /// 현재 UTC 기준으로 최근 <c>hours</c> 시간의 데이터를 반환한다.
    /// 취득 건수가 0인 시간대는 결과에 포함되지 않는다.
    /// </para>
    ///
    /// <para>
    /// <b>모드 2 — 기간 지정:</b> <c>from</c> 파라미터를 지정하면 해당 UTC 시각부터
    /// <c>to</c> (생략 시 현재 UTC)까지의 데이터를 반환한다.
    /// </para>
    /// </summary>
    /// <param name="hours">최근 몇 시간을 조회할지 (모드 1, 기본값 24, 최대 168).</param>
    /// <param name="from">조회 시작 시각 (UTC ISO-8601, 모드 2).</param>
    /// <param name="to">조회 종료 시각 (UTC ISO-8601, 모드 2, 생략 시 현재).</param>
    [HttpGet("hourly")]
    public async Task<ActionResult> GetHourlyStats(
        [FromQuery] int hours = 24,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        IReadOnlyList<CaptureStat> stats;

        if (from.HasValue)
        {
            // 모드 2: 기간 지정
            var fromUtc = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
            var toUtc   = to.HasValue
                ? DateTime.SpecifyKind(to.Value, DateTimeKind.Utc)
                : DateTime.UtcNow;

            if (fromUtc > toUtc)
                return BadRequest(new { error = "'from' must be earlier than 'to'" });

            stats = await _captureStatService.GetRangeAsync(fromUtc, toUtc);
        }
        else
        {
            // 모드 1: 최근 N시간
            var clampedHours = Math.Clamp(hours, 1, 168); // max 7일
            stats = await _captureStatService.GetRecentHoursAsync(clampedHours);
        }

        return Ok(stats.Select(s => new
        {
            hourUtc = s.HourUtc,
            count   = s.Count,
        }));
    }

    /// <summary>
    /// 오늘 자정(UTC 기준) 이후 현재까지의 총 취득 건수를 반환한다.
    /// </summary>
    /// <returns><c>{ "count": 42 }</c> 형식의 JSON 객체.</returns>
    [HttpGet("today")]
    public async Task<ActionResult> GetTodayCount()
    {
        var count = await _captureStatService.GetTodayCountAsync();
        return Ok(new { count });
    }

    /// <summary>
    /// 시간별 취득 건수 집계 결과를 반환한다.
    ///
    /// <para>
    /// <b>모드 1 — 최근 N시간:</b> <c>hours</c> 파라미터만 지정 (기본값 24).
    /// 현재 UTC 기준으로 최근 <c>hours</c> 시간 데이터와 전체 합산 건수를 반환한다.
    /// 취득 건수가 0인 시간대는 결과에 포함되지 않는다.
    /// </para>
    ///
    /// <para>
    /// <b>모드 2 — 기간 지정:</b> <c>from</c> 파라미터를 지정하면 해당 UTC 시각부터
    /// <c>to</c> (생략 시 현재 UTC)까지의 데이터와 합산 건수를 반환한다.
    /// </para>
    ///
    /// <para>응답 예시:</para>
    /// <code>
    /// {
    ///   "totalCount": 125,
    ///   "buckets": [
    ///     { "hourUtc": "2024-01-01T10:00:00Z", "count": 15 },
    ///     { "hourUtc": "2024-01-01T11:00:00Z", "count": 30 }
    ///   ]
    /// }
    /// </code>
    /// </summary>
    /// <param name="hours">최근 몇 시간을 조회할지 (모드 1, 기본값 24, 최대 168).</param>
    /// <param name="from">조회 시작 시각 (UTC ISO-8601, 모드 2).</param>
    /// <param name="to">조회 종료 시각 (UTC ISO-8601, 모드 2, 생략 시 현재).</param>
    [HttpGet("captures")]
    public async Task<ActionResult> GetCaptureStats(
        [FromQuery] int hours = 24,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        IReadOnlyList<CaptureStat> stats;

        if (from.HasValue)
        {
            // 모드 2: 기간 지정
            var fromUtc = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
            var toUtc   = to.HasValue
                ? DateTime.SpecifyKind(to.Value, DateTimeKind.Utc)
                : DateTime.UtcNow;

            if (fromUtc > toUtc)
                return BadRequest(new { error = "'from' must be earlier than 'to'" });

            stats = await _captureStatService.GetRangeAsync(fromUtc, toUtc);
        }
        else
        {
            // 모드 1: 최근 N시간
            var clampedHours = Math.Clamp(hours, 1, 168); // max 7일
            stats = await _captureStatService.GetRecentHoursAsync(clampedHours);
        }

        var buckets = stats.Select(s => new
        {
            hourUtc = s.HourUtc,
            count   = s.Count,
        }).ToList();

        return Ok(new
        {
            totalCount = buckets.Sum(b => b.count),
            buckets,
        });
    }
}
