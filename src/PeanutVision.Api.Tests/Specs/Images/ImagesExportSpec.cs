using System.IO.Compression;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Images;

public class ImagesExportSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    private Guid _id1;
    private Guid _id2;
    private Guid _id3;
    private readonly List<string> _tempFiles = [];

    public ImagesExportSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICapturedImageRepository>();

        _id1 = Guid.NewGuid();
        _id2 = Guid.NewGuid();
        _id3 = Guid.NewGuid();

        var file1 = CreateTempFile("export_img1.png");
        var file2 = CreateTempFile("export_img2.png");
        var file3 = CreateTempFile("export_img3.png");

        await repo.AddAsync(new CapturedImage
        {
            Id = _id1,
            FilePath = file1,
            Format = "png",
            Width = 10,
            Height = 10,
            FileSizeBytes = new FileInfo(file1).Length,
            CapturedAt = DateTime.UtcNow.AddMinutes(-3),
        });

        await repo.AddAsync(new CapturedImage
        {
            Id = _id2,
            FilePath = file2,
            Format = "png",
            Width = 10,
            Height = 10,
            FileSizeBytes = new FileInfo(file2).Length,
            CapturedAt = DateTime.UtcNow.AddMinutes(-2),
        });

        await repo.AddAsync(new CapturedImage
        {
            Id = _id3,
            FilePath = file3,
            Format = "png",
            Width = 10,
            Height = 10,
            FileSizeBytes = new FileInfo(file3).Length,
            CapturedAt = DateTime.UtcNow.AddMinutes(-1),
        });
    }

    public Task DisposeAsync()
    {
        foreach (var f in _tempFiles)
        {
            try { File.Delete(f); } catch { /* ignore */ }
        }
        return Task.CompletedTask;
    }

    private string CreateTempFile(string name)
    {
        var path = Path.Combine(Path.GetTempPath(), $"peanut_export_{Guid.NewGuid():N}_{name}");
        // Write minimal valid PNG bytes so the file exists and is non-empty
        File.WriteAllBytes(path, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00]);
        _tempFiles.Add(path);
        return path;
    }

    [Fact]
    public async Task export_with_specific_ids_returns_zip()
    {
        var response = await _client.PostJsonAsync("/api/images/export", new { ids = new[] { _id1, _id2 } });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/zip", response.Content.Headers.ContentType?.MediaType);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        using var zip = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        Assert.Equal(2, zip.Entries.Count);
    }

    [Fact]
    public async Task export_empty_ids_exports_all_images()
    {
        var response = await _client.PostJsonAsync("/api/images/export", new { ids = Array.Empty<Guid>() });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        using var zip = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        Assert.True(zip.Entries.Count >= 3, $"Expected at least 3 entries but got {zip.Entries.Count}");
    }

    [Fact]
    public async Task export_with_nonexistent_ids_skips_missing()
    {
        var nonExistentId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var response = await _client.PostJsonAsync("/api/images/export",
            new { ids = new[] { _id1, nonExistentId } });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        using var zip = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        Assert.Single(zip.Entries);
    }

    [Fact]
    public async Task export_single_image_zip_has_correct_filename()
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICapturedImageRepository>();
        var image = await repo.GetByIdAsync(_id1);
        var expectedFilename = Path.GetFileName(image!.FilePath);

        var response = await _client.PostJsonAsync("/api/images/export", new { ids = new[] { _id1 } });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        using var zip = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        Assert.Single(zip.Entries);
        Assert.Equal(expectedFilename, zip.Entries[0].Name);
    }

    [Fact]
    public async Task export_response_is_streaming_zip_can_be_opened()
    {
        var response = await _client.PostJsonAsync("/api/images/export", new { ids = new[] { _id1, _id2, _id3 } });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 0);

        using var zip = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        Assert.Equal(3, zip.Entries.Count);
    }
}
