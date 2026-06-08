using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Tests.Acquisition;

public class AcquisitionLatestFrameTests : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public AcquisitionLatestFrameTests(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task LatestFrame_when_no_frame_returns_no_content()
    {
        var response = await _client.GetAsync("/api/acquisition/latest-frame");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task LatestFrame_after_trigger_returns_png()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });

        // Trigger is fire-and-forget (202); poll until frame arrives
        await _client.PostAsync("/api/acquisition/trigger", null);

        HttpResponseMessage response = null!;
        for (var i = 0; i < 20; i++)
        {
            response = await _client.GetAsync("/api/acquisition/latest-frame");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                break;
            }
            await Task.Delay(100);
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 8);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'N', bytes[2]);
        Assert.Equal((byte)'G', bytes[3]);
    }

}
