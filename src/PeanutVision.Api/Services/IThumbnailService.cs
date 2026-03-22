namespace PeanutVision.Api.Services;

public interface IThumbnailService
{
    /// <summary>
    /// Generates a JPEG thumbnail for the given image file.
    /// Returns the thumbnail path, or null if the format is unsupported or generation fails.
    /// </summary>
    Task<string?> GenerateAsync(string sourceFilePath);
}
