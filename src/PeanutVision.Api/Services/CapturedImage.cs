namespace PeanutVision.Api.Services;

public sealed class CapturedImage
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSizeBytes { get; set; }
    public string Format { get; set; } = string.Empty;  // "png", "bmp", "raw"
    public DateTime CapturedAt { get; set; }

    /// <summary>Tags stored as a JSON array string, e.g. "[\"defect\",\"blur\"]"</summary>
    public string Tags { get; set; } = "[]";

    /// <summary>Freeform annotation notes for this capture</summary>
    public string Notes { get; set; } = string.Empty;
}
