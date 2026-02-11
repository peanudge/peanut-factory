using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

public class AcquisitionTriggerSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public AcquisitionTriggerSpec(PeanutVisionApiFactory factory)
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
    public async Task Trigger_when_inactive_returns_409()
    {
        var response = await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Trigger_when_active_returns_ok()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("message", out _));
    }

    [Fact]
    public async Task Trigger_increments_hal_trigger_count()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        _factory.ResetMockState();

        await _client.PostAsync("/api/acquisition/trigger", null);
        await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(2, _factory.MockHal.CallLog.SoftwareTriggerCount);
    }
}
