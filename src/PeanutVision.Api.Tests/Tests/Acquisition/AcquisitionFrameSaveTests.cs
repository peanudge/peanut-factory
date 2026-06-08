using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Tests.Acquisition;

/// <summary>
/// Verifies the headless save flow that replaced the removed Snapshot endpoint:
/// POST /start {frameCount:1} → frame arrives → FrameSaveService saves to disk + DB.
/// </summary>
public class AcquisitionFrameSaveTests : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public AcquisitionFrameSaveTests(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
        await _client.DeleteAsync("/api/acquisition");
    }

    [Fact]
    public async Task Start_with_frameCount_1_triggers_autosave_in_db()
    {
        // Count images before
        var before = await GetImageCount();

        // Start soft-trigger acquisition for exactly 1 frame
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam", frameCount = 1 });

        // Fire the trigger (MockHAL simulates a frame immediately)
        await _client.PostAsync("/api/acquisition/trigger", null);

        // FrameSaveService.SaveAsync is fire-and-forget — poll until record appears
        var saved = await PollAsync(
            condition: async () => await GetImageCount() > before,
            timeoutMs: 3000);

        Assert.True(saved, "FrameSaveService should have written a DB record within 3 seconds");
    }

    [Fact]
    public async Task Start_with_frameCount_1_channel_returns_to_idle_after_frame()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam", frameCount = 1 });

        await _client.PostAsync("/api/acquisition/trigger", null);

        // Channel should auto-stop after 1 frame
        var stopped = await PollAsync(
            condition: async () =>
            {
                var res = await _client.GetAsync("/api/acquisition/status");
                using var doc = await res.ReadJsonDocumentAsync();
                return !doc.RootElement.GetProperty("isActive").GetBoolean();
            },
            timeoutMs: 3000);

        Assert.True(stopped, "Channel should become inactive after frameCount=1 completes");
    }

    [Fact]
    public async Task Start_with_frameCount_1_then_start_again_succeeds()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam", frameCount = 1 });
        await _client.PostAsync("/api/acquisition/trigger", null);

        // Wait for channel to go idle
        await PollAsync(
            condition: async () =>
            {
                var res = await _client.GetAsync("/api/acquisition/status");
                using var doc = await res.ReadJsonDocumentAsync();
                return !doc.RootElement.GetProperty("isActive").GetBoolean();
            },
            timeoutMs: 3000);

        // Should be able to start again immediately
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam", frameCount = 1 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<int> GetImageCount()
    {
        var res = await _client.GetAsync("/api/images?page=1&pageSize=1");
        if (!res.IsSuccessStatusCode) return 0;
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("totalCount").GetInt32();
    }

    private static async Task<bool> PollAsync(Func<Task<bool>> condition, int timeoutMs)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            if (await condition()) return true;
            await Task.Delay(100);
        }
        return false;
    }
}
