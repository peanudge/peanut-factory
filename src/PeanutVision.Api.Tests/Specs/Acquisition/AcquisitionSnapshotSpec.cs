using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

public class AcquisitionSnapshotSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public AcquisitionSnapshotSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task Snapshot_when_idle_returns_png_image()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Snapshot_response_contains_valid_png_bytes()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var bytes = await response.Content.ReadAsByteArrayAsync();

        // PNG magic bytes: 0x89 P N G
        Assert.True(bytes.Length > 8);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'N', bytes[2]);
        Assert.Equal((byte)'G', bytes[3]);
    }

    [Fact]
    public async Task Snapshot_response_has_content_disposition()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var disposition = response.Content.Headers.ContentDisposition;
        Assert.NotNull(disposition);
        Assert.Equal("snapshot.png", disposition.FileName);
    }

    [Fact]
    public async Task Snapshot_when_active_returns_conflict()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Snapshot_with_unknown_profile_returns_not_found()
    {
        var response = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "nonexistent-profile" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Snapshot_can_be_called_sequentially()
    {
        var response1 = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        var response2 = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
    }

    [Fact]
    public async Task Snapshot_does_not_affect_acquisition_status()
    {
        await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var statusResponse = await _client.GetAsync("/api/acquisition/status");
        using var doc = await statusResponse.ReadJsonDocumentAsync();

        Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
    }
}
