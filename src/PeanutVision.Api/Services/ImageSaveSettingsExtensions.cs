using PeanutVision.Capture;

namespace PeanutVision.Api.Services;

internal static class ImageSaveSettingsExtensions
{
    public static FrameWriterOptions ToWriterOptions(this ImageSaveSettings settings, string contentRootPath)
        => new()
        {
            OutputDirectory = Path.IsPathRooted(settings.OutputDirectory)
                ? settings.OutputDirectory
                : Path.Combine(contentRootPath, settings.OutputDirectory),
            // FilenamePrefix, TimestampFormat, Format use FrameWriterOptions defaults (capture_{timestamp}.png)
        };
}
