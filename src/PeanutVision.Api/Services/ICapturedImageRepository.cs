namespace PeanutVision.Api.Services;

public interface ICapturedImageRepository
{
    Task<CapturedImage> AddAsync(CapturedImage image);
    Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
        int page, int pageSize,
        DateOnly? date = null);
    Task<CapturedImage?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
}
