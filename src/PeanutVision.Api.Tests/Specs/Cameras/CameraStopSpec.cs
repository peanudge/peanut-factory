using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraStopSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraStopSpec(PeanutVisionApiFactory factory) { _factory = factory; _client = factory.CreateClient(); }
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Stop_when_idle_returns_ok()
    {
        var response = await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("message", out _));
    }

    [Fact]
    public async Task Stop_when_active_stops_acquisition()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var status = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await status.ReadJsonDocumentAsync();
        Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Stop_sets_hal_acquisition_stopped()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        _factory.ResetMockState();

        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

        Assert.True(_factory.MockHal.CallLog.AcquisitionStopped);
    }
}
