using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Tests.Acquisition;

/// <summary>
/// Verifies GET /api/images date range filter (replaced session filter).
/// </summary>
public class ImagesDateFilterTests : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public ImagesDateFilterTests(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
        await _client.DeleteAsync("/api/acquisition");
    }

    private async Task SaveOneFrame()
    {
        // Snapshot count before so we can wait for it to increase (handles shared DB across tests)
        var before = await GetImageCount();

        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam", frameCount = 1 });
        await _client.PostAsync("/api/acquisition/trigger", null);

        // Wait for AutoSaveService to write a new DB record
        await PollAsync(
            condition: async () => await GetImageCount() > before,
            timeoutMs: 8000);
    }

    [Fact]
    public async Task GetImages_with_no_filter_returns_all_images()
    {
        await SaveOneFrame();

        var count = await GetImageCount();
        Assert.True(count > 0, "At least one image should be in DB after SaveOneFrame");
    }

    [Fact]
    public async Task GetImages_dateFrom_future_returns_empty()
    {
        await SaveOneFrame();

        var tomorrow = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/images?dateFrom={tomorrow}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(0, doc.RootElement.GetProperty("totalCount").GetInt32());
    }

    [Fact]
    public async Task GetImages_dateTo_past_returns_empty()
    {
        await SaveOneFrame();

        var yesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/images?dateTo={yesterday}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(0, doc.RootElement.GetProperty("totalCount").GetInt32());
    }

    [Fact]
    public async Task GetImages_dateFrom_yesterday_includes_today_images()
    {
        await SaveOneFrame();

        var yesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/images?dateFrom={yesterday}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(doc.RootElement.GetProperty("totalCount").GetInt32() > 0);
    }

    [Fact]
    public async Task GetImages_response_has_no_sessionId_field()
    {
        await SaveOneFrame();

        var response = await _client.GetAsync("/api/images");
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();

        Assert.NotEmpty(items);
        Assert.False(items[0].TryGetProperty("sessionId", out _),
            "sessionId field must not exist in image records");
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
