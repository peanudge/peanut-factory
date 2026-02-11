using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

public class AcquisitionLifecycleSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public AcquisitionLifecycleSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task Full_lifecycle_start_status_stop_status()
    {
        // Start
        var startResponse = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        // Status while active
        var activeStatus = await _client.GetAsync("/api/acquisition/status");
        using (var doc = await activeStatus.ReadJsonDocumentAsync())
        {
            Assert.True(doc.RootElement.GetProperty("isActive").GetBoolean());
        }

        // Stop
        var stopResponse = await _client.PostAsync("/api/acquisition/stop", null);
        Assert.Equal(HttpStatusCode.OK, stopResponse.StatusCode);

        // Status after stop
        var idleStatus = await _client.GetAsync("/api/acquisition/status");
        using (var doc = await idleStatus.ReadJsonDocumentAsync())
        {
            Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
        }
    }

    [Fact]
    public async Task Can_restart_after_stop()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        await _client.PostAsync("/api/acquisition/stop", null);

        // Second start with different profile
        var response = await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal("crevis-tc-a160k-softtrig-rgb8", doc.RootElement.GetProperty("profileId").GetString());
    }

    [Fact]
    public async Task Trigger_after_stop_returns_409()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        await _client.PostAsync("/api/acquisition/stop", null);

        var response = await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
