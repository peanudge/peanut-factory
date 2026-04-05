using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;
using System.Net;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

/// <summary>
/// 취득(Capture) 이벤트 발생 시 CaptureStatService 가 시간별 카운트를
/// 올바르게 집계하는지 검증하는 통합 테스트.
/// </summary>
public class CaptureStatFlowSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public CaptureStatFlowSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    // ---------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------

    private async Task<int> GetCurrentHourCountAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db   = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hour = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month,
                               DateTime.UtcNow.Day,  DateTime.UtcNow.Hour, 0, 0, DateTimeKind.Utc);

        var stat = await db.CaptureStats.AsNoTracking()
                           .FirstOrDefaultAsync(s => s.HourUtc == hour);
        return stat?.Count ?? 0;
    }

    // ---------------------------------------------------------------------------
    // Tests
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task snapshot_increments_hourly_capture_count()
    {
        var before = await GetCurrentHourCountAsync();

        var snapResponse = await _client.PostJsonAsync(
            "/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.OK, snapResponse.StatusCode);

        var after = await GetCurrentHourCountAsync();
        Assert.True(after > before,
            $"Hourly capture count should increase from {before} after snapshot, got {after}");
    }

    [Fact]
    public async Task trigger_increments_hourly_capture_count_when_autosave_enabled()
    {
        // Verify AutoSave is on
        var settingsResponse = await _client.GetAsync("/api/settings/image-save");
        Assert.Equal(HttpStatusCode.OK, settingsResponse.StatusCode);
        using var settingsDoc = await settingsResponse.ReadJsonDocumentAsync();
        var autoSave = settingsDoc.RootElement.GetProperty("autoSave").GetBoolean();
        if (!autoSave)
        {
            // Skip if AutoSave is off — stat is only recorded when image is saved
            return;
        }

        var before = await GetCurrentHourCountAsync();

        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });

        var triggerResponse = await _client.PostAsync("/api/acquisition/trigger", null);
        Assert.Equal(HttpStatusCode.OK, triggerResponse.StatusCode);

        var after = await GetCurrentHourCountAsync();
        Assert.True(after > before,
            $"Hourly capture count should increase from {before} after trigger, got {after}");
    }

    [Fact]
    public async Task multiple_snapshots_accumulate_count_in_same_hour_bucket()
    {
        var before = await GetCurrentHourCountAsync();
        const int snapshotCount = 3;

        for (var i = 0; i < snapshotCount; i++)
        {
            var r = await _client.PostJsonAsync(
                "/api/acquisition/snapshot",
                new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
            Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        }

        var after = await GetCurrentHourCountAsync();
        Assert.True(after >= before + snapshotCount,
            $"Expected count to increase by at least {snapshotCount}, " +
            $"was {before}, now {after}");
    }
}
