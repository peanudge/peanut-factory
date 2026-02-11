using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

public class AcquisitionStopSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public AcquisitionStopSpec(PeanutVisionApiFactory factory)
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
    public async Task Stop_when_idle_returns_ok()
    {
        var response = await _client.PostAsync("/api/acquisition/stop", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("message", out _));
    }

    [Fact]
    public async Task Stop_when_active_returns_ok_and_stops_acquisition()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PostAsync("/api/acquisition/stop", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify status shows idle
        var statusResponse = await _client.GetAsync("/api/acquisition/status");
        using var doc = await statusResponse.ReadJsonDocumentAsync();
        Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Stop_sets_hal_acquisition_stopped()
    {
        _factory.ResetMockState();

        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        await _client.PostAsync("/api/acquisition/stop", null);

        Assert.True(_factory.MockHal.CallLog.AcquisitionStopped);
    }
}
