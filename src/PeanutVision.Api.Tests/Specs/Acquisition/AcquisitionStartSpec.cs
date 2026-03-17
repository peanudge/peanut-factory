using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

public class AcquisitionStartSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public AcquisitionStartSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task Start_with_valid_profile_returns_ok()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", doc.RootElement.GetProperty("profileId").GetString());
    }

    [Fact]
    public async Task Start_with_unknown_profile_returns_404()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "nonexistent-camera" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("error", out _));
    }

    [Fact]
    public async Task Start_when_already_active_returns_409()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Start_sets_hal_acquisition_started()
    {
        _factory.ResetMockState();

        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.True(_factory.MockHal.CallLog.AcquisitionStarted);
    }

    [Fact]
    public async Task Start_with_custom_trigger_mode_returns_ok()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam", triggerMode = "MC_TrigMode_SOFT" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Start_response_contains_message()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("message", out var msg));
        Assert.False(string.IsNullOrEmpty(msg.GetString()));
    }

    [Fact]
    public async Task Start_with_frameCount_and_intervalMs_returns_ok()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam", frameCount = 10, intervalMs = 100 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Start_with_intervalMs_below_minimum_returns_bad_request()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam", intervalMs = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("error", out _));
    }

    [Fact]
    public async Task Start_with_intervalMs_zero_returns_ok()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam", intervalMs = 0 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
