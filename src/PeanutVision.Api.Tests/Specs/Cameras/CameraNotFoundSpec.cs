using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

/// <summary>
/// Verifies that all /api/cameras/{cameraId}/... endpoints return 404
/// when the camera ID is not registered.
/// </summary>
public class CameraNotFoundSpec : IClassFixture<PeanutVisionApiFactory>
{
    private readonly HttpClient _client;
    private const string UnknownCamId = "nonexistent-cam";

    public CameraNotFoundSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Stop_with_unknown_camera_returns_404()
    {
        var response = await _client.PostAsync($"/api/cameras/{UnknownCamId}/stop", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Status_with_unknown_camera_returns_404()
    {
        var response = await _client.GetAsync($"/api/cameras/{UnknownCamId}/status");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Trigger_with_unknown_camera_returns_404()
    {
        var response = await _client.PostAsync($"/api/cameras/{UnknownCamId}/trigger", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task LatestFrame_with_unknown_camera_returns_404()
    {
        var response = await _client.GetAsync($"/api/cameras/{UnknownCamId}/latest-frame");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Histogram_with_unknown_camera_returns_404()
    {
        var response = await _client.GetAsync($"/api/cameras/{UnknownCamId}/latest-frame/histogram");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Snapshot_with_unknown_camera_returns_404()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{UnknownCamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetExposure_with_unknown_camera_returns_404()
    {
        var response = await _client.GetAsync($"/api/cameras/{UnknownCamId}/exposure");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SetExposure_with_unknown_camera_returns_404()
    {
        var response = await _client.PutJsonAsync($"/api/cameras/{UnknownCamId}/exposure",
            new { exposureUs = 10000.0 });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
