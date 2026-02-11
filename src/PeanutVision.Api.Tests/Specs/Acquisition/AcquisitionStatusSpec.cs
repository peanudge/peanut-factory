using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

public class AcquisitionStatusSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public AcquisitionStatusSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task Status_when_idle_shows_inactive()
    {
        var response = await _client.GetAsync("/api/acquisition/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
        Assert.Equal(JsonValueKind.Null, doc.RootElement.GetProperty("profileId").ValueKind);
    }

    [Fact]
    public async Task Status_when_active_shows_active_with_profile()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.GetAsync("/api/acquisition/status");

        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.GetProperty("isActive").GetBoolean());
        Assert.Equal("crevis-tc-a160k-freerun-rgb8", doc.RootElement.GetProperty("profileId").GetString());
    }

    [Fact]
    public async Task Status_includes_hasFrame_field()
    {
        var response = await _client.GetAsync("/api/acquisition/status");

        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("hasFrame", out _));
    }

    [Fact]
    public async Task Status_when_active_includes_statistics_shape()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.GetAsync("/api/acquisition/status");

        using var doc = await response.ReadJsonDocumentAsync();
        var stats = doc.RootElement.GetProperty("statistics");
        Assert.NotEqual(JsonValueKind.Null, stats.ValueKind);
        Assert.True(stats.TryGetProperty("frameCount", out _));
        Assert.True(stats.TryGetProperty("droppedFrameCount", out _));
        Assert.True(stats.TryGetProperty("errorCount", out _));
        Assert.True(stats.TryGetProperty("elapsedMs", out _));
        Assert.True(stats.TryGetProperty("averageFps", out _));
    }
}
