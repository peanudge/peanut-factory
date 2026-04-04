using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraTriggerSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraTriggerSpec(PeanutVisionApiFactory factory) { _factory = factory; _client = factory.CreateClient(); }
    public async Task InitializeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Trigger_when_inactive_returns_409()
    {
        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Trigger_when_active_returns_png_image()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Trigger_response_contains_valid_png_bytes()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 8);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, bytes[..8]);
    }

    [Fact]
    public async Task Trigger_increments_hal_trigger_count()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        _factory.ResetMockState();
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(2, _factory.MockHal.CallLog.SoftwareTriggerCount);
    }

    [Fact]
    public async Task Trigger_returns_image_path_header()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Image-Path"),
            "Trigger response should include X-Image-Path header");
        var savedPath = response.Headers.GetValues("X-Image-Path").First();
        Assert.True(File.Exists(savedPath), $"Saved image should exist at {savedPath}");
    }
}
