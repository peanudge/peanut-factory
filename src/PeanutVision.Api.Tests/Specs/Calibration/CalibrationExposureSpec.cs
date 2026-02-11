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
    public async Task GetExposure_without_active_channel_returns_409()
    {
        var response = await _client.GetAsync("/api/calibration/exposure");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task SetExposure_without_active_channel_returns_409()
    {
        var response = await _client.PutJsonAsync("/api/calibration/exposure",
            new { exposureUs = 5000.0 });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // --- With active channel ---

    [Fact]
    public async Task GetExposure_returns_exposure_and_gain()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.GetAsync("/api/calibration/exposure");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("exposureUs", out _));
        Assert.True(doc.RootElement.TryGetProperty("gainDb", out _));
    }

    [Fact]
    public async Task GetExposure_includes_range()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

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
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PutJsonAsync("/api/calibration/exposure",
            new { exposureUs = 5000.0 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(5000.0, doc.RootElement.GetProperty("exposureUs").GetDouble());
    }

    [Fact]
    public async Task SetGain_updates_gain_value()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PutJsonAsync("/api/calibration/exposure",
            new { gainDb = 3.5 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(3.5, doc.RootElement.GetProperty("gainDb").GetDouble());
    }

    [Fact]
    public async Task SetExposure_and_gain_together()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PutJsonAsync("/api/calibration/exposure",
            new { exposureUs = 20000.0, gainDb = 1.5 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(20000.0, doc.RootElement.GetProperty("exposureUs").GetDouble());
        Assert.Equal(1.5, doc.RootElement.GetProperty("gainDb").GetDouble());
    }
}
