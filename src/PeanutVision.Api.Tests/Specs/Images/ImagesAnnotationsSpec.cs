using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Images;

public class ImagesAnnotationsSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private Guid _imageId;

    public ImagesAnnotationsSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICapturedImageRepository>();

        _imageId = Guid.NewGuid();
        await repo.AddAsync(new CapturedImage
        {
            Id = _imageId,
            FilePath = "/tmp/annotatable.png",
            Format = "png",
            Width = 100,
            Height = 100,
            FileSizeBytes = 500,
            CapturedAt = DateTime.UtcNow,
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Patch_annotations_returns_updated_dto_with_tags_and_notes()
    {
        var payload = new { tags = new[] { "defect", "blur" }, notes = "Needs review" };
        var response = await _client.PatchJsonAsync($"/api/images/{_imageId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var tags = doc.RootElement.GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ToArray();
        var notes = doc.RootElement.GetProperty("notes").GetString();
        Assert.Contains("defect", tags);
        Assert.Contains("blur", tags);
        Assert.Equal("Needs review", notes);
    }

    [Fact]
    public async Task Patch_annotations_returns_404_for_unknown_id()
    {
        var payload = new { tags = new[] { "test" }, notes = "" };
        var response = await _client.PatchJsonAsync($"/api/images/{Guid.NewGuid()}", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Patch_annotations_empty_tags_and_notes_clears_previous()
    {
        // First set some annotations
        await _client.PatchJsonAsync($"/api/images/{_imageId}", new { tags = new[] { "tag1" }, notes = "some note" });

        // Now clear them
        var response = await _client.PatchJsonAsync($"/api/images/{_imageId}", new { tags = Array.Empty<string>(), notes = "" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var tags = doc.RootElement.GetProperty("tags").EnumerateArray().ToList();
        var notes = doc.RootElement.GetProperty("notes").GetString();
        Assert.Empty(tags);
        Assert.Equal("", notes);
    }

    [Fact]
    public async Task GetImages_response_includes_tags_and_notes_fields()
    {
        var response = await _client.GetAsync("/api/images?pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.NotEmpty(items);
        // Every item should have tags and notes fields
        foreach (var item in items)
        {
            Assert.True(item.TryGetProperty("tags", out _), "Expected 'tags' field in image DTO");
            Assert.True(item.TryGetProperty("notes", out _), "Expected 'notes' field in image DTO");
        }
    }
}
