using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraHistogramSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraHistogramSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Histogram_when_no_frame_returns_no_content()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame/histogram");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Histogram_after_trigger_returns_ok_with_rgb_arrays()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);

        var response = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame/histogram");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("red", out var red));
        Assert.True(doc.RootElement.TryGetProperty("green", out var green));
        Assert.True(doc.RootElement.TryGetProperty("blue", out var blue));
        Assert.Equal(256, doc.RootElement.GetProperty("bins").GetInt32());
        Assert.Equal(256, red.GetArrayLength());
        Assert.Equal(256, green.GetArrayLength());
        Assert.Equal(256, blue.GetArrayLength());
    }

    [Fact]
    public async Task Histogram_values_are_non_negative_integers()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);

        var response = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame/histogram");
        using var doc = await response.ReadJsonDocumentAsync();

        var red = doc.RootElement.GetProperty("red").EnumerateArray().Select(e => e.GetInt32()).ToList();
        Assert.All(red, v => Assert.True(v >= 0));
        Assert.True(red.Sum() > 0, "Histogram should have at least some non-zero values");
    }
}
