using Microsoft.AspNetCore.Mvc;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/filesystem")]
public class FilesystemController : ControllerBase
{
    [HttpGet("defaults")]
    public ActionResult GetDefaults()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        return Ok(new { desktopPath = desktop });
    }

    [HttpGet("roots")]
    public ActionResult GetRoots()
    {
        var roots = Environment.OSVersion.Platform == PlatformID.Win32NT
            ? DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .Select(d => d.RootDirectory.FullName)
                .ToList()
            : ["/"];

        return Ok(roots);
    }

    [HttpGet("list")]
    public ActionResult ListDirectory([FromQuery] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest(new { error = "path is required" });

        if (!Directory.Exists(path))
            return NotFound(new { error = $"Directory not found: {path}" });

        try
        {
            var entries = Directory.GetDirectories(path)
                .Select(d => new DirectoryEntry(
                    Path.GetFileName(d),
                    d,
                    HasSubdirectories(d)))
                .OrderBy(e => e.Name)
                .ToList();

            return Ok(entries);
        }
        catch (UnauthorizedAccessException)
        {
            return Ok(Array.Empty<DirectoryEntry>());
        }
    }

    [HttpGet("validate")]
    public ActionResult ValidatePath([FromQuery] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest(new { error = "path is required" });

        try
        {
            Directory.CreateDirectory(path);
            var testFile = Path.Combine(path, $".pv_write_test_{Guid.NewGuid():N}");
            System.IO.File.WriteAllText(testFile, "");
            System.IO.File.Delete(testFile);
            return Ok(new { writable = true, path });
        }
        catch
        {
            return Ok(new { writable = false, path });
        }
    }

    private static bool HasSubdirectories(string path)
    {
        try { return Directory.EnumerateDirectories(path).Any(); }
        catch { return false; }
    }
}

public record DirectoryEntry(string Name, string Path, bool HasChildren);
