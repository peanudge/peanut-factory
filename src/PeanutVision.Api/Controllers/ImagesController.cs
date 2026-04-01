using System.IO.Compression;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly ICapturedImageRepository _repo;

    public ImagesController(ICapturedImageRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<ImagePageDto>> GetImages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? sessionId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] string? format = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var (items, total) = await _repo.GetPageAsync(page, pageSize, sessionId, dateFrom, dateTo, format);
        return Ok(new ImagePageDto(
            items.Select(ToDto).ToList(),
            total,
            page,
            pageSize,
            (int)Math.Ceiling(total / (double)pageSize)));
    }

    [HttpGet("{id:guid}/thumbnail")]
    public async Task<IActionResult> GetThumbnail(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image is null) return NotFound();
        if (image.ThumbnailPath is null || !System.IO.File.Exists(image.ThumbnailPath))
            return NotFound(new { error = "Thumbnail not available" });
        return PhysicalFile(image.ThumbnailPath, "image/jpeg");
    }

    [HttpGet("{id:guid}/file")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image is null) return NotFound();
        if (!System.IO.File.Exists(image.FilePath))
            return NotFound(new { error = "File not found on disk" });

        var mimeType = image.Format switch
        {
            "bmp" => "image/bmp",
            "raw" => "application/octet-stream",
            _ => "image/png",
        };
        return PhysicalFile(image.FilePath, mimeType, Path.GetFileName(image.FilePath));
    }

    [HttpPost("export")]
    public async Task<IActionResult> ExportZip([FromBody] ExportRequest request)
    {
        IReadOnlyList<CapturedImage> images;

        if (request.Ids is { Count: > 0 })
        {
            var tasks = request.Ids.Select(id => _repo.GetByIdAsync(id));
            var results = await Task.WhenAll(tasks);
            images = results.Where(r => r is not null).Select(r => r!).ToList();
        }
        else
        {
            // Export all — paginate through everything
            var allImages = new List<CapturedImage>();
            int page = 1;
            const int pageSize = 200;
            while (true)
            {
                var (pageItems, total) = await _repo.GetPageAsync(page, pageSize);
                allImages.AddRange(pageItems);
                if (allImages.Count >= total) break;
                page++;
            }
            images = allImages;
        }

        Response.ContentType = "application/zip";
        Response.Headers.ContentDisposition = "attachment; filename=\"peanut-vision-export.zip\"";

        using var zip = new ZipArchive(Response.BodyWriter.AsStream(leaveOpen: true), ZipArchiveMode.Create, leaveOpen: true);
        var usedNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var img in images)
        {
            if (!System.IO.File.Exists(img.FilePath)) continue;

            var baseName = Path.GetFileName(img.FilePath);
            string entryName;
            if (usedNames.TryGetValue(baseName, out int count))
            {
                usedNames[baseName] = count + 1;
                var ext = Path.GetExtension(baseName);
                entryName = Path.GetFileNameWithoutExtension(baseName) + $"_{count + 1}" + ext;
            }
            else
            {
                usedNames[baseName] = 1;
                entryName = baseName;
            }

            var entry = zip.CreateEntry(entryName, CompressionLevel.NoCompression);
            await using var entryStream = entry.Open();
            await using var fileStream = System.IO.File.OpenRead(img.FilePath);
            await fileStream.CopyToAsync(entryStream);
        }

        return new EmptyResult();
    }

    [HttpGet("{id:guid}/histogram")]
    public async Task<IActionResult> GetHistogram(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image is null) return NotFound();
        if (!System.IO.File.Exists(image.FilePath))
            return NotFound(new { error = "File not found on disk" });

        int[] red = new int[256];
        int[] green = new int[256];
        int[] blue = new int[256];

        using var img = await SixLabors.ImageSharp.Image.LoadAsync<Rgb24>(image.FilePath);
        img.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                foreach (ref readonly var px in row)
                {
                    red[px.R]++;
                    green[px.G]++;
                    blue[px.B]++;
                }
            }
        });

        // Normalize to 0–1 range
        int maxCount = 1;
        for (int i = 0; i < 256; i++)
        {
            if (red[i] > maxCount) maxCount = red[i];
            if (green[i] > maxCount) maxCount = green[i];
            if (blue[i] > maxCount) maxCount = blue[i];
        }

        double scale = 1.0 / maxCount;
        var redNorm = red.Select(v => Math.Round(v * scale, 6)).ToArray();
        var greenNorm = green.Select(v => Math.Round(v * scale, 6)).ToArray();
        var blueNorm = blue.Select(v => Math.Round(v * scale, 6)).ToArray();

        return Ok(new { red = redNorm, green = greenNorm, blue = blueNorm, bins = 256 });
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<CapturedImageDto>> PatchAnnotations(Guid id, [FromBody] PatchAnnotationsRequest request)
    {
        var tagsJson = JsonSerializer.Serialize(request.Tags ?? []);
        var updated = await _repo.UpdateAnnotationsAsync(id, tagsJson, request.Notes ?? string.Empty);
        if (updated is null) return NotFound();
        return Ok(ToDto(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image is null) return NotFound();

        try { if (System.IO.File.Exists(image.FilePath)) System.IO.File.Delete(image.FilePath); } catch { }
        try
        {
            if (image.ThumbnailPath is not null && System.IO.File.Exists(image.ThumbnailPath))
                System.IO.File.Delete(image.ThumbnailPath);
        }
        catch { }

        await _repo.DeleteAsync(id);
        return NoContent();
    }

    private static CapturedImageDto ToDto(CapturedImage c)
    {
        string[] tags;
        try { tags = JsonSerializer.Deserialize<string[]>(c.Tags) ?? []; }
        catch { tags = []; }
        return new CapturedImageDto(
            c.Id,
            c.FilePath,
            Path.GetFileName(c.FilePath),
            c.ThumbnailPath is not null,
            c.Width,
            c.Height,
            c.FileSizeBytes,
            c.Format,
            c.CapturedAt,
            c.SessionId,
            tags,
            c.Notes);
    }
}

public record CapturedImageDto(
    Guid Id,
    string FilePath,
    string Filename,
    bool HasThumbnail,
    int Width,
    int Height,
    long FileSizeBytes,
    string Format,
    DateTime CapturedAt,
    Guid? SessionId,
    string[] Tags,
    string Notes);

public record ImagePageDto(
    IReadOnlyList<CapturedImageDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

public record ExportRequest(IReadOnlyList<Guid>? Ids);

public record PatchAnnotationsRequest(string[]? Tags, string? Notes);
