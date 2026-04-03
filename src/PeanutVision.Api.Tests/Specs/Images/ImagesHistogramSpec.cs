using System.Net;
using Microsoft.Extensions.DependencyInjection;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PeanutVision.Api.Tests.Specs.Images;

public class ImagesHistogramSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private Guid _imageId;
    private string _tempFilePath = string.Empty;

    public ImagesHistogramSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Create a small 4x4 PNG with known pixel values
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"hist-test-{Guid.NewGuid():N}.png");

        using (var img = new Image<Rgb24>(4, 4))
        {
            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        // All pixels are pure red: R=255, G=0, B=0
                        row[x] = new Rgb24(255, 0, 0);
                    }
                }
            });
            await img.SaveAsPngAsync(_tempFilePath);
        }

        _imageId = Guid.NewGuid();
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICapturedImageRepository>();
        await repo.AddAsync(new CapturedImage
        {
            Id = _imageId,
            FilePath = _tempFilePath,
            Format = "png",
            Width = 4,
            Height = 4,
            FileSizeBytes = new FileInfo(_tempFilePath).Length,
            CapturedAt = DateTime.UtcNow,
        });
    }

    public Task DisposeAsync()
    {
        try { File.Delete(_tempFilePath); } catch { /* ignore */ }
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetHistogram_returns_200_with_three_channels_of_256_bins()
    {
        var response = await _client.GetAsync($"/api/images/{_imageId}/histogram");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();

        Assert.True(doc.RootElement.TryGetProperty("red", out var redEl));
        Assert.True(doc.RootElement.TryGetProperty("green", out var greenEl));
        Assert.True(doc.RootElement.TryGetProperty("blue", out var blueEl));
        Assert.Equal(256, redEl.GetArrayLength());
        Assert.Equal(256, greenEl.GetArrayLength());
        Assert.Equal(256, blueEl.GetArrayLength());
    }

    [Fact]
    public async Task GetHistogram_pure_red_image_has_red_channel_peak_at_255()
    {
        var response = await _client.GetAsync($"/api/images/{_imageId}/histogram");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();

        // All pixels are R=255, so red[255] should be 1.0 (normalized max)
        var redArr = doc.RootElement.GetProperty("red").EnumerateArray().ToArray();
        Assert.Equal(1.0, redArr[255].GetDouble(), precision: 5);

        // Green and blue channels should all be 0 (all pixels have G=0, B=0)
        var greenArr = doc.RootElement.GetProperty("green").EnumerateArray().ToArray();
        Assert.Equal(1.0, greenArr[0].GetDouble(), precision: 5);

        var blueArr = doc.RootElement.GetProperty("blue").EnumerateArray().ToArray();
        Assert.Equal(1.0, blueArr[0].GetDouble(), precision: 5);
    }

    [Fact]
    public async Task GetHistogram_bins_field_is_256()
    {
        var response = await _client.GetAsync($"/api/images/{_imageId}/histogram");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(256, doc.RootElement.GetProperty("bins").GetInt32());
    }

    [Fact]
    public async Task GetHistogram_returns_404_for_unknown_id()
    {
        var response = await _client.GetAsync($"/api/images/{Guid.NewGuid()}/histogram");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetHistogram_normalized_values_are_between_0_and_1()
    {
        var response = await _client.GetAsync($"/api/images/{_imageId}/histogram");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();

        foreach (var channel in new[] { "red", "green", "blue" })
        {
            var arr = doc.RootElement.GetProperty(channel).EnumerateArray();
            foreach (var element in arr)
            {
                var v = element.GetDouble();
                Assert.True(v >= 0.0 && v <= 1.0, $"{channel} bin value {v} out of [0,1] range");
            }
        }
    }
}
