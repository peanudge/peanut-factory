namespace PeanutVision.Api.Services;

public interface ICapturedImageRepository
{
    Task<CapturedImage> AddAsync(CapturedImage image);
    Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
        int page, int pageSize,
        Guid? sessionId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? format = null);
    Task<CapturedImage?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
}
