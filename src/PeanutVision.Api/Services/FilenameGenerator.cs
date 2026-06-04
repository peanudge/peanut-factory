namespace PeanutVision.Api.Services;

public sealed class FilenameGenerator
{
    private int _sequenceCounter;

    /// <summary>
    /// outputDirectory 내 지원 토큰:
    ///   {date}    → yyyy-MM-dd
    ///   {profile} → profileId에서 특수문자를 _로 치환한 값
    /// 파일명 고정: capture_yyyyMMdd_HHmmss_fff_NNNNN.ext
    /// </summary>
    public string Generate(ImageSaveSettings settings, string contentRootPath, string? profileId = null)
    {
        var now = DateTime.Now;
        var dir = ExpandDirectory(settings.OutputDirectory, contentRootPath, now, profileId);
        Directory.CreateDirectory(dir);

        var seq = Interlocked.Increment(ref _sequenceCounter);
        var ext = settings.Format switch
        {
            SaveImageFormat.Bmp => ".bmp",
            SaveImageFormat.Raw => ".raw",
            _ => ".png",
        };
        var name = $"capture_{now:yyyyMMdd_HHmmss_fff}_{seq:D5}{ext}";
        return Path.Combine(dir, name);
    }

    private static string ExpandDirectory(string template, string contentRootPath, DateTime now, string? profileId)
    {
        if (string.IsNullOrWhiteSpace(template))
            return Path.Combine(contentRootPath, "CapturedImages");

        var expanded = template
            .Replace("{date}", now.ToString("yyyy-MM-dd"))
            .Replace("{profile}", SanitizeSegment(profileId ?? "unknown"));

        return Path.IsPathRooted(expanded)
            ? expanded
            : Path.Combine(contentRootPath, expanded);
    }

    private static string SanitizeSegment(string segment) =>
        string.Concat(segment.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
}
