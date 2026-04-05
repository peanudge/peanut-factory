using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraListSpec : IClassFixture<PeanutVisionApiFactory>
{
    private readonly HttpClient _client;
    public CameraListSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task List_returns_ok_with_cameras_array()
    {
        var response = await _client.GetAsync("/api/cameras");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var cameras = doc.RootElement.GetProperty("cameras").EnumerateArray().ToList();
        Assert.NotEmpty(cameras);
        Assert.Equal("cam-1", cameras[0].GetProperty("id").GetString());
    }
}
