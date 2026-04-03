namespace PeanutVision.Api.Services;

public interface ICapturedImageRepository
{
    Task<CapturedImage> AddAsync(CapturedImage image);
    Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
        int page, int pageSize,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? format = null);
    Task<CapturedImage?> GetByIdAsync(Guid id);
    Task<CapturedImage?> UpdateAnnotationsAsync(Guid id, string tagsJson, string notes);
    Task DeleteAsync(Guid id);
}
