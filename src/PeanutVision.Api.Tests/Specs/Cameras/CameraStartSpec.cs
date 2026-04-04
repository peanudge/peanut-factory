using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraStartSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraStartSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
    }

    [Fact]
    public async Task Start_with_valid_profile_returns_ok()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", doc.RootElement.GetProperty("profileId").GetString());
    }

    [Fact]
    public async Task Start_with_unknown_profile_returns_404()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "nonexistent-camera" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Start_when_already_active_returns_409()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Start_with_unknown_camera_id_returns_404()
    {
        var response = await _client.PostJsonAsync("/api/cameras/nonexistent-cam/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Start_sets_hal_acquisition_started()
    {
        _factory.ResetMockState();
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.True(_factory.MockHal.CallLog.AcquisitionStarted);
    }

    [Fact]
    public async Task Start_with_intervalMs_below_minimum_returns_bad_request()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam", intervalMs = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
