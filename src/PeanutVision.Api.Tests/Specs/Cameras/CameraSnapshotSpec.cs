using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraSnapshotSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraSnapshotSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Snapshot_when_idle_returns_png_image()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Snapshot_response_contains_valid_png_bytes()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 8);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'N', bytes[2]);
        Assert.Equal((byte)'G', bytes[3]);
    }

    [Fact]
    public async Task Snapshot_when_active_returns_conflict()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Snapshot_with_unknown_profile_returns_not_found()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "nonexistent-profile" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Snapshot_can_be_called_sequentially()
    {
        var r1 = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var r2 = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, r1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, r2.StatusCode);
    }

    [Fact]
    public async Task Snapshot_does_not_affect_acquisition_status()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var statusResponse = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await statusResponse.ReadJsonDocumentAsync();
        Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
    }
}
