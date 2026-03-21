using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Calibration;

public class CalibrationExposureSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public CalibrationExposureSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    // --- No active channel ---

    [Fact]
    public async Task GetExposure_without_active_channel_returns_desired_defaults()
    {
        var response = await _client.GetAsync("/api/calibration/exposure");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("exposureUs", out var exposureEl));
        Assert.True(exposureEl.GetDouble() > 0);
    }

    [Fact]
    public async Task SetExposure_without_active_channel_stores_desired_value()
    {
        var response = await _client.PutJsonAsync("/api/calibration/exposure",
            new { exposureUs = 5000.0 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(5000.0, doc.RootElement.GetProperty("exposureUs").GetDouble());
    }

    // --- With active channel ---

    [Fact]
    public async Task GetExposure_returns_exposure_and_range_when_active()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.GetAsync("/api/calibration/exposure");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("exposureUs", out _));
        Assert.False(doc.RootElement.TryGetProperty("gainDb", out _));
    }

    [Fact]
    public async Task GetExposure_includes_range()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.GetAsync("/api/calibration/exposure");

        using var doc = await response.ReadJsonDocumentAsync();
        var range = doc.RootElement.GetProperty("exposureRange");
        Assert.True(range.TryGetProperty("min", out _));
        Assert.True(range.TryGetProperty("max", out _));
    }

    [Fact]
    public async Task SetExposure_updates_exposure_value()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.PutJsonAsync("/api/calibration/exposure",
            new { exposureUs = 5000.0 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(5000.0, doc.RootElement.GetProperty("exposureUs").GetDouble());
    }

    [Fact]
    public async Task SetExposure_persists_across_get()
    {
        await _client.PutJsonAsync("/api/calibration/exposure", new { exposureUs = 8000.0 });

        var response = await _client.GetAsync("/api/calibration/exposure");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(8000.0, doc.RootElement.GetProperty("exposureUs").GetDouble());
    }
}
