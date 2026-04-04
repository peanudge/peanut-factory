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
            FilenamePrefix = string.IsNullOrWhiteSpace(settings.FilenamePrefix) ? "frame" : settings.FilenamePrefix,
            TimestampFormat = settings.TimestampFormat,
            Format = settings.Format switch
            {
                SaveImageFormat.Bmp => OutputFormat.Bmp,
                SaveImageFormat.Raw => OutputFormat.Raw,
                _ => OutputFormat.Png,
            },
        };
}
