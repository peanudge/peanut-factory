using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraExposureSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraExposureSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task GetExposure_returns_ok_with_exposureUs()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/exposure");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("exposureUs", out _));
    }

    [Fact]
    public async Task SetExposure_updates_value()
    {
        await _client.PutJsonAsync($"/api/cameras/{CamId}/exposure", new { exposureUs = 15000.0 });
        var response = await _client.GetAsync($"/api/cameras/{CamId}/exposure");
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(15000.0, doc.RootElement.GetProperty("exposureUs").GetDouble(), precision: 1);
    }
}
