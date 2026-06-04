namespace PeanutVision.Api.Services;

public sealed class CapturedImage
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSizeBytes { get; set; }
    public string Format { get; set; } = string.Empty;
    public DateTime CapturedAt { get; set; }
}
