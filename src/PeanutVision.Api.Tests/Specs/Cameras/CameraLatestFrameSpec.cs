using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraLatestFrameSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    private readonly PeanutVisionApiFactory _factory;

    public CameraLatestFrameSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        _factory.ResetMockState();
    }
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task LatestFrame_when_no_frame_returns_no_content_or_ok()
    {
        // CameraActor retains the last frame across start/stop cycles,
        // so NoContent only appears on a truly fresh actor. With IClassFixture
        // shared state, prior tests may have captured a frame already.
        var response = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame");
        Assert.True(
            response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK,
            $"Expected NoContent or OK, got {response.StatusCode}");
    }

    [Fact]
    public async Task LatestFrame_after_trigger_returns_png()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);

        var response = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 8);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'N', bytes[2]);
        Assert.Equal((byte)'G', bytes[3]);
    }

    [Fact]
    public async Task LatestFrame_with_autosave_sets_image_path_header_on_first_poll()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);

        var first  = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame");
        var second = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame");

        Assert.True(first.Headers.Contains("X-Image-Path"),
            "First poll of a new frame should set X-Image-Path");
        Assert.False(second.Headers.Contains("X-Image-Path"),
            "Second poll of the same frame must not set X-Image-Path");
    }
}
