using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Images;

public class ImagesFilterSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public ImagesFilterSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Seed test images with known capturedAt and format values
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICapturedImageRepository>();

        await repo.AddAsync(new CapturedImage
        {
            Id = Guid.NewGuid(),
            FilePath = "/tmp/img1.png",
            Format = "png",
            Width = 100,
            Height = 100,
            FileSizeBytes = 1000,
            CapturedAt = new DateTime(2025, 1, 10, 12, 0, 0, DateTimeKind.Utc),
        });

        await repo.AddAsync(new CapturedImage
        {
            Id = Guid.NewGuid(),
            FilePath = "/tmp/img2.bmp",
            Format = "bmp",
            Width = 100,
            Height = 100,
            FileSizeBytes = 2000,
            CapturedAt = new DateTime(2025, 3, 15, 12, 0, 0, DateTimeKind.Utc),
        });

        await repo.AddAsync(new CapturedImage
        {
            Id = Guid.NewGuid(),
            FilePath = "/tmp/img3.raw",
            Format = "raw",
            Width = 100,
            Height = 100,
            FileSizeBytes = 3000,
            CapturedAt = new DateTime(2025, 6, 20, 12, 0, 0, DateTimeKind.Utc),
        });

        await repo.AddAsync(new CapturedImage
        {
            Id = Guid.NewGuid(),
            FilePath = "/tmp/img4.png",
            Format = "png",
            Width = 100,
            Height = 100,
            FileSizeBytes = 1500,
            CapturedAt = new DateTime(2025, 9, 5, 12, 0, 0, DateTimeKind.Utc),
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetImages_without_filters_returns_all_seeded_images()
    {
        var response = await _client.GetAsync("/api/images");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var total = doc.RootElement.GetProperty("totalCount").GetInt32();
        Assert.True(total >= 4);
    }

    [Fact]
    public async Task GetImages_format_png_returns_only_png_images()
    {
        var response = await _client.GetAsync("/api/images?format=png&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 2);
        Assert.All(items, item =>
            Assert.Equal("png", item.GetProperty("format").GetString()));
    }

    [Fact]
    public async Task GetImages_format_bmp_returns_only_bmp_images()
    {
        var response = await _client.GetAsync("/api/images?format=bmp&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 1);
        Assert.All(items, item =>
            Assert.Equal("bmp", item.GetProperty("format").GetString()));
    }

    [Fact]
    public async Task GetImages_format_raw_returns_only_raw_images()
    {
        var response = await _client.GetAsync("/api/images?format=raw&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 1);
        Assert.All(items, item =>
            Assert.Equal("raw", item.GetProperty("format").GetString()));
    }

    [Fact]
    public async Task GetImages_dateFrom_excludes_images_before_date()
    {
        var response = await _client.GetAsync("/api/images?dateFrom=2025-06-01&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 2);
        Assert.All(items, item =>
        {
            var capturedAt = item.GetProperty("capturedAt").GetDateTime();
            Assert.True(capturedAt >= new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        });
    }

    [Fact]
    public async Task GetImages_dateTo_excludes_images_after_date()
    {
        var response = await _client.GetAsync("/api/images?dateTo=2025-04-01&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 2);
        Assert.All(items, item =>
        {
            var capturedAt = item.GetProperty("capturedAt").GetDateTime();
            Assert.True(capturedAt <= new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc));
        });
    }

    [Fact]
    public async Task GetImages_date_range_filters_correctly()
    {
        var response = await _client.GetAsync("/api/images?dateFrom=2025-02-01&dateTo=2025-07-01&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 2);
        Assert.All(items, item =>
        {
            var capturedAt = item.GetProperty("capturedAt").GetDateTime();
            Assert.True(capturedAt >= new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc));
            Assert.True(capturedAt <= new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc));
        });
    }

    [Fact]
    public async Task GetImages_format_and_dateFrom_combined_filters_correctly()
    {
        var response = await _client.GetAsync("/api/images?format=png&dateFrom=2025-08-01&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.True(items.Count >= 1);
        Assert.All(items, item =>
        {
            Assert.Equal("png", item.GetProperty("format").GetString());
            var capturedAt = item.GetProperty("capturedAt").GetDateTime();
            Assert.True(capturedAt >= new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc));
        });
    }

    [Fact]
    public async Task GetImages_unknown_format_returns_empty_items()
    {
        var response = await _client.GetAsync("/api/images?format=tiff&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var total = doc.RootElement.GetProperty("totalCount").GetInt32();
        Assert.Equal(0, total);
    }

    [Fact]
    public async Task GetImages_response_includes_totalCount_page_pageSize_totalPages()
    {
        var response = await _client.GetAsync("/api/images?format=png");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("totalCount", out _));
        Assert.True(doc.RootElement.TryGetProperty("page", out _));
        Assert.True(doc.RootElement.TryGetProperty("pageSize", out _));
        Assert.True(doc.RootElement.TryGetProperty("totalPages", out _));
    }
}
