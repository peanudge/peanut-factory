namespace PeanutVision.Api.Services;

public sealed class FilenameGenerator
{
    private int _sequenceCounter;

    public string Generate(ImageSaveSettings settings, string contentRootPath, string? profileId = null)
    {
        var now = DateTime.Now;
        var baseDir = ResolveDirectory(settings.OutputDirectory, contentRootPath);

        var subdir = settings.SubfolderStrategy switch
        {
            SubfolderStrategy.ByDate => now.ToString("yyyy-MM-dd"),
            SubfolderStrategy.ByProfile when !string.IsNullOrEmpty(profileId) => SanitizeSegment(profileId),
            _ => null,
        };

        var dir = subdir is not null ? Path.Combine(baseDir, subdir) : baseDir;
        Directory.CreateDirectory(dir);

        var prefix = string.IsNullOrWhiteSpace(settings.FilenamePrefix) ? "capture" : settings.FilenamePrefix;
        var ext = settings.Format switch
        {
            SaveImageFormat.Bmp => ".bmp",
            SaveImageFormat.Raw => ".raw",
            _ => ".png",
        };

        string name;
        if (settings.IncludeSequenceNumber)
        {
            var seq = Interlocked.Increment(ref _sequenceCounter);
            name = $"{prefix}_{now.ToString(settings.TimestampFormat)}_{seq:D5}{ext}";
        }
        else
        {
            name = $"{prefix}_{now.ToString(settings.TimestampFormat)}{ext}";
        }

        return Path.Combine(dir, name);
    }

    private static string ResolveDirectory(string outputDir, string contentRootPath)
    {
        if (string.IsNullOrWhiteSpace(outputDir))
            return Path.Combine(contentRootPath, "CapturedImages");
        return Path.IsPathRooted(outputDir) ? outputDir : Path.Combine(contentRootPath, outputDir);
    }

    private static string SanitizeSegment(string segment) =>
        string.Concat(segment.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
}
