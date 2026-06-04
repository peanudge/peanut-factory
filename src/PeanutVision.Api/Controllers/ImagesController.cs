using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;

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
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var (items, total) = await _repo.GetPageAsync(page, pageSize, dateFrom, dateTo);
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

    private static CapturedImageDto ToDto(CapturedImage c) => new(
        c.Id,
        c.FilePath,
        Path.GetFileName(c.FilePath),
        c.ThumbnailPath is not null,
        c.Width,
        c.Height,
        c.FileSizeBytes,
        c.Format,
        c.CapturedAt);
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
    DateTime CapturedAt);

public record ImagePageDto(
    IReadOnlyList<CapturedImageDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
