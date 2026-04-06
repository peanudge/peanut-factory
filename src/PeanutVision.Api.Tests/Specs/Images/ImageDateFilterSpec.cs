using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Images;

public class ImageDateFilterSpec : IClassFixture<PeanutVisionApiFactory>
{
    private readonly HttpClient _client;

    public ImageDateFilterSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_images_with_today_date_returns_ok()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/images?date={today}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_images_with_future_date_returns_empty_list()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/images?date={future}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(0, doc.RootElement.GetProperty("totalCount").GetInt32());
    }

    [Fact]
    public async Task Get_images_with_no_date_returns_all()
    {
        var response = await _client.GetAsync("/api/images");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("items", out _));
    }

    [Fact]
    public async Task SessionId_param_no_longer_accepted()
    {
        var response = await _client.GetAsync("/api/images?sessionId=00000000-0000-0000-0000-000000000000");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
