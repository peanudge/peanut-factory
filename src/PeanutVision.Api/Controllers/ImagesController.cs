using Microsoft.AspNetCore.Mvc;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly string _contentRootPath;

    public ImagesController(IWebHostEnvironment environment)
    {
        _contentRootPath = environment.ContentRootPath;
    }

    [HttpGet("file")]
    public IActionResult GetFile([FromQuery] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest(new { error = "path is required", errorCode = "INVALID_PARAMETER" });

        string resolved = Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(path, _contentRootPath);

        // Directory traversal guard
        string normalizedRoot = Path.GetFullPath(_contentRootPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        if (!resolved.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            return StatusCode(403, new { error = "Access to this path is not permitted.", errorCode = "ACCESS_DENIED" });

        if (!System.IO.File.Exists(resolved))
            return NotFound(new { error = $"File not found: {Path.GetFileName(resolved)}", errorCode = "RESOURCE_NOT_FOUND" });

        string contentType = Path.GetExtension(resolved).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".bmp" => "image/bmp",
            _      => "application/octet-stream",
        };

        var stream = new FileStream(resolved, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(stream, contentType, Path.GetFileName(resolved));
    }
}
