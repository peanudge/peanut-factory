using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraLifecycleSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraLifecycleSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Full_lifecycle_start_status_stop_status()
    {
        var startResponse = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        using (var doc = await (await _client.GetAsync($"/api/cameras/{CamId}/status")).ReadJsonDocumentAsync())
            Assert.True(doc.RootElement.GetProperty("isActive").GetBoolean());

        var stopResponse = await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        Assert.Equal(HttpStatusCode.OK, stopResponse.StatusCode);

        using (var doc = await (await _client.GetAsync($"/api/cameras/{CamId}/status")).ReadJsonDocumentAsync())
            Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Can_restart_after_stop()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal("crevis-tc-a160k-softtrig-rgb8.cam", doc.RootElement.GetProperty("profileId").GetString());
    }

    [Fact]
    public async Task Trigger_after_stop_returns_409()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
