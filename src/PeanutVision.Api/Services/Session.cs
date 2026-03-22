namespace PeanutVision.Api.Services;

public sealed class Session
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string? Notes { get; set; }

    public bool IsActive => EndedAt is null;

    public ICollection<CapturedImage> CapturedImages { get; set; } = [];
}
