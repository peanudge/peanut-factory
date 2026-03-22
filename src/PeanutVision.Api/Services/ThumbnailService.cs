using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace PeanutVision.Api.Services;

public sealed class ThumbnailService(ILogger<ThumbnailService> logger) : IThumbnailService
{
    private const int MaxWidth = 256;
    private const int MaxHeight = 192;

    public async Task<string?> GenerateAsync(string sourceFilePath)
    {
        var ext = Path.GetExtension(sourceFilePath).ToLowerInvariant();
        if (ext is ".raw")
            return null;

        try
        {
            var dir = Path.Combine(Path.GetDirectoryName(sourceFilePath)!, ".thumbnails");
            Directory.CreateDirectory(dir);

            var thumbName = Path.GetFileNameWithoutExtension(sourceFilePath) + ".jpg";
            var thumbPath = Path.Combine(dir, thumbName);

            using var image = await Image.LoadAsync(sourceFilePath);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(MaxWidth, MaxHeight),
                Mode = ResizeMode.Max,
            }));
            await image.SaveAsJpegAsync(thumbPath, new JpegEncoder { Quality = 85 });

            return thumbPath;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Thumbnail generation failed for {Path}", sourceFilePath);
            return null;
        }
    }
}
