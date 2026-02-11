using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

public class AcquisitionCaptureSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public AcquisitionCaptureSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task Capture_when_no_acquisition_returns_404()
    {
        var response = await _client.PostAsync("/api/acquisition/capture", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Capture_when_active_but_no_frame_returns_404()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        // No simulated frame acquisition, so LastFrame is null
        var response = await _client.PostAsync("/api/acquisition/capture", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
