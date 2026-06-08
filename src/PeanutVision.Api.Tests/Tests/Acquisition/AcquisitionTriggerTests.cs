using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Tests.Acquisition;

public class AcquisitionTriggerTests : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public AcquisitionTriggerTests(PeanutVisionApiFactory factory)
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
    public async Task Trigger_when_active_returns_202()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });

        var response = await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task Trigger_increments_hal_trigger_count()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        _factory.ResetMockState();

        // Triggers are sequential now (each awaits the frame)
        await _client.PostAsync("/api/acquisition/trigger", null);
        await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(2, _factory.MockHal.CallLog.SoftwareTriggerCount);
    }

}
