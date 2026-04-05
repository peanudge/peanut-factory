using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Stats;

/// <summary>
/// GET /api/stats/captures 엔드포인트 통합 테스트.
/// 시간별 취득 건수 집계 결과를 totalCount + buckets 구조로 반환하는 기능을 검증한다.
/// </summary>
public class StatsCapturesSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public StatsCapturesSpec(PeanutVisionApiFactory factory)
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
    public async Task GetCaptureStats_returns_ok()
    {
        var response = await _client.GetAsync("/api/stats/captures");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCaptureStats_returns_object_with_totalCount_and_buckets()
    {
        var response = await _client.GetAsync("/api/stats/captures");

        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);
        Assert.True(doc.RootElement.TryGetProperty("totalCount", out _),
            "Response must have 'totalCount' field");
        Assert.True(doc.RootElement.TryGetProperty("buckets", out var buckets),
            "Response must have 'buckets' field");
        Assert.Equal(JsonValueKind.Array, buckets.ValueKind);
    }

    [Fact]
    public async Task GetCaptureStats_totalCount_is_non_negative()
    {
        var response = await _client.GetAsync("/api/stats/captures");

        using var doc = await response.ReadJsonDocumentAsync();
        var totalCount = doc.RootElement.GetProperty("totalCount").GetInt32();
        Assert.True(totalCount >= 0, "totalCount must be non-negative");
    }

    [Fact]
    public async Task GetCaptureStats_hours_param_is_clamped_to_1_at_minimum()
    {
        var response = await _client.GetAsync("/api/stats/captures?hours=0");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCaptureStats_hours_param_is_clamped_to_168_at_maximum()
    {
        var response = await _client.GetAsync("/api/stats/captures?hours=9999");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // -------------------------------------------------------------------------
    // 스냅샷 취득 후 통계 반영 검증
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetCaptureStats_totalCount_increments_after_snapshot()
    {
        var totalBefore = await GetTotalCountAsync();

        var snapResponse = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, snapResponse.StatusCode);

        var totalAfter = await GetTotalCountAsync();
        Assert.True(totalAfter > totalBefore,
            $"Expected totalCount to increase from {totalBefore}, but got {totalAfter}");
    }

    [Fact]
    public async Task GetCaptureStats_buckets_have_hourUtc_and_count_fields()
    {
        // 최소 한 건의 취득 데이터가 있어야 필드 검증 가능
        await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.GetAsync("/api/stats/captures?hours=1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = await response.ReadJsonDocumentAsync();
        var buckets = doc.RootElement.GetProperty("buckets").EnumerateArray().ToList();

        Assert.NotEmpty(buckets);

        var first = buckets[0];
        Assert.True(first.TryGetProperty("hourUtc", out var hourUtc),
            "Bucket item must have 'hourUtc' field");
        Assert.True(first.TryGetProperty("count", out var count),
            "Bucket item must have 'count' field");

        Assert.True(DateTime.TryParse(hourUtc.GetString(), out _),
            "hourUtc must be a parseable DateTime string");
        Assert.True(count.GetInt32() > 0, "count must be positive");
    }

    [Fact]
    public async Task GetCaptureStats_totalCount_equals_sum_of_bucket_counts()
    {
        await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.GetAsync("/api/stats/captures?hours=24");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = await response.ReadJsonDocumentAsync();
        var totalCount = doc.RootElement.GetProperty("totalCount").GetInt32();
        var bucketSum  = doc.RootElement.GetProperty("buckets")
                            .EnumerateArray()
                            .Sum(b => b.GetProperty("count").GetInt32());

        Assert.Equal(bucketSum, totalCount);
    }

    // -------------------------------------------------------------------------
    // 기간 지정 모드 (from / to)
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetCaptureStats_range_mode_returns_ok()
    {
        var from = DateTime.UtcNow.AddHours(-2).ToString("o");
        var to   = DateTime.UtcNow.ToString("o");

        var response = await _client.GetAsync(
            $"/api/stats/captures?from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCaptureStats_range_mode_returns_bad_request_when_from_after_to()
    {
        var from = DateTime.UtcNow.ToString("o");
        var to   = DateTime.UtcNow.AddHours(-1).ToString("o");  // to < from

        var response = await _client.GetAsync(
            $"/api/stats/captures?from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCaptureStats_range_mode_with_from_only_defaults_to_now()
    {
        var from = DateTime.UtcNow.AddHours(-1).ToString("o");

        var response = await _client.GetAsync(
            $"/api/stats/captures?from={Uri.EscapeDataString(from)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async Task<int> GetTotalCountAsync()
    {
        var response = await _client.GetAsync("/api/stats/captures?hours=1");
        response.EnsureSuccessStatusCode();
        using var doc = await response.ReadJsonDocumentAsync();
        return doc.RootElement.GetProperty("totalCount").GetInt32();
    }
}
