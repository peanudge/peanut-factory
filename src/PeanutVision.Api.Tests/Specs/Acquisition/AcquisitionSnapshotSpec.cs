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
    public async Task Snapshot_when_active_returns_conflict()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
