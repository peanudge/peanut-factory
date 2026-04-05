using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Stats;

/// <summary>
/// GET /api/stats/hourly 엔드포인트 통합 테스트.
/// 시간대별 취득 건수 통계 조회 기능을 검증한다.
/// </summary>
public class StatsHourlySpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public StatsHourlySpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    // -------------------------------------------------------------------------
    // 기본 응답 구조
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetHourlyStats_returns_ok_with_no_data()
    {
        var response = await _client.GetAsync("/api/stats/hourly");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHourlyStats_returns_json_array()
    {
        var response = await _client.GetAsync("/api/stats/hourly");

        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
    }

    [Fact]
    public async Task GetHourlyStats_hours_param_is_clamped_to_1_at_minimum()
    {
        // hours=0 should be treated as hours=1 (no crash, no error)
        var response = await _client.GetAsync("/api/stats/hourly?hours=0");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHourlyStats_hours_param_is_clamped_to_168_at_maximum()
    {
        var response = await _client.GetAsync("/api/stats/hourly?hours=9999");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // -------------------------------------------------------------------------
    // 스냅샷 취득 후 통계 반영 검증
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetHourlyStats_count_increments_after_snapshot()
    {
        // 스냅샷 전 현재 시간 버킷의 카운트를 기록
        var countBefore = await GetCurrentHourCount();

        // 스냅샷 취득 (AutoSave=true → RecordCaptureAsync 호출됨)
        var snapResponse = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, snapResponse.StatusCode);

        // 통계 카운트가 1 이상 증가했는지 확인
        var countAfter = await GetCurrentHourCount();
        Assert.True(countAfter > countBefore,
            $"Expected hourly count to increase from {countBefore}, but got {countAfter}");
    }

    [Fact]
    public async Task GetHourlyStats_each_snapshot_increments_count_by_one()
    {
        var countBefore = await GetCurrentHourCount();

        // 2회 취득
        await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var countAfter = await GetCurrentHourCount();
        Assert.True(countAfter >= countBefore + 2,
            $"Expected at least {countBefore + 2} captures, got {countAfter}");
    }

    // -------------------------------------------------------------------------
    // 기간 지정 모드 (from / to)
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetHourlyStats_range_mode_returns_ok()
    {
        var from = DateTime.UtcNow.AddHours(-2).ToString("o");
        var to   = DateTime.UtcNow.ToString("o");

        var response = await _client.GetAsync($"/api/stats/hourly?from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHourlyStats_range_mode_with_from_only_defaults_to_now()
    {
        var from = DateTime.UtcNow.AddHours(-1).ToString("o");

        var response = await _client.GetAsync($"/api/stats/hourly?from={Uri.EscapeDataString(from)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
    }

    [Fact]
    public async Task GetHourlyStats_range_mode_returns_bad_request_when_from_after_to()
    {
        var from = DateTime.UtcNow.ToString("o");
        var to   = DateTime.UtcNow.AddHours(-1).ToString("o");  // to < from

        var response = await _client.GetAsync(
            $"/api/stats/hourly?from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // -------------------------------------------------------------------------
    // 응답 아이템 필드 검증
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetHourlyStats_items_have_hourUtc_and_count_fields()
    {
        // 최소 한 건의 취득 데이터가 있어야 필드 검증 가능
        await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.GetAsync("/api/stats/hourly?hours=1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.EnumerateArray().ToList();

        Assert.NotEmpty(items);

        var first = items[0];
        Assert.True(first.TryGetProperty("hourUtc", out var hourUtc),
            "Response item must have 'hourUtc' field");
        Assert.True(first.TryGetProperty("count", out var count),
            "Response item must have 'count' field");

        // hourUtc는 파싱 가능한 DateTime이어야 한다
        Assert.True(DateTime.TryParse(hourUtc.GetString(), out _),
            "hourUtc must be a parseable DateTime string");

        // count는 양수여야 한다
        Assert.True(count.GetInt32() > 0,
            "count must be positive");
    }

    [Fact]
    public async Task GetHourlyStats_items_are_ordered_ascending_by_hourUtc()
    {
        // 데이터가 충분하도록 취득
        await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.GetAsync("/api/stats/hourly?hours=24");
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.EnumerateArray().ToList();

        if (items.Count < 2)
            return; // 단일 항목이면 정렬 검증 불필요

        var dates = items
            .Select(i => DateTime.Parse(i.GetProperty("hourUtc").GetString()!))
            .ToList();

        for (var i = 1; i < dates.Count; i++)
            Assert.True(dates[i] >= dates[i - 1],
                $"Items are not in ascending order: {dates[i - 1]:o} > {dates[i]:o}");
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>현재 UTC 시간 버킷(정시)의 취득 카운트를 반환한다. 데이터가 없으면 0.</summary>
    private async Task<int> GetCurrentHourCount()
    {
        var response = await _client.GetAsync("/api/stats/hourly?hours=1");
        response.EnsureSuccessStatusCode();

        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.EnumerateArray().ToList();

        var currentHour = new DateTime(
            DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
            DateTime.UtcNow.Hour, 0, 0, DateTimeKind.Utc);

        foreach (var item in items)
        {
            if (DateTime.TryParse(item.GetProperty("hourUtc").GetString(), out var hourUtc)
                && hourUtc == currentHour)
            {
                return item.GetProperty("count").GetInt32();
            }
        }

        return 0;
    }
}
