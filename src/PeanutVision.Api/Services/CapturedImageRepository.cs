using Microsoft.EntityFrameworkCore;

namespace PeanutVision.Api.Services;

public sealed class CapturedImageRepository : ICapturedImageRepository
{
    private readonly AppDbContext _db;

    public CapturedImageRepository(AppDbContext db) => _db = db;

    public async Task<CapturedImage> AddAsync(CapturedImage image)
    {
        _db.CapturedImages.Add(image);
        await _db.SaveChangesAsync();
        return image;
    }

    public async Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
        int page, int pageSize,
        DateOnly? date = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _db.CapturedImages.AsNoTracking().AsQueryable();

        if (date.HasValue)
        {
            var from = date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var to   = date.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            query = query.Where(c => c.CapturedAt >= from && c.CapturedAt <= to);
        }
        else
        {
            if (dateFrom.HasValue)
                query = query.Where(c => c.CapturedAt >= dateFrom.Value);
            if (dateTo.HasValue)
                query = query.Where(c => c.CapturedAt <= dateTo.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.CapturedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public Task<CapturedImage?> GetByIdAsync(Guid id)
        => _db.CapturedImages.FindAsync(id).AsTask()!;

    public async Task DeleteAsync(Guid id)
    {
        var image = await _db.CapturedImages.FindAsync(id)
            ?? throw new KeyNotFoundException($"CapturedImage {id} not found");
        _db.CapturedImages.Remove(image);
        await _db.SaveChangesAsync();
    }
}
