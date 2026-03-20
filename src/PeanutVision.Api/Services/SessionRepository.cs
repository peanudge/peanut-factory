using Microsoft.EntityFrameworkCore;

namespace PeanutVision.Api.Services;

public sealed class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _db;

    public SessionRepository(AppDbContext db) => _db = db;

    public async Task<Session> CreateAsync(string name, string? notes = null)
    {
        var session = new Session
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow,
            Notes = notes,
        };

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();
        return session;
    }

    public async Task<Session?> GetByIdAsync(Guid id)
        => await _db.Sessions.FindAsync(id);

    public async Task<IReadOnlyList<Session>> GetAllAsync(int limit = 50)
        => await _db.Sessions
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
            .ToListAsync();

    public async Task<Session?> GetActiveAsync()
        => await _db.Sessions
            .Where(s => s.EndedAt == null)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<Session> EndSessionAsync(Guid id)
    {
        var session = await _db.Sessions.FindAsync(id)
            ?? throw new KeyNotFoundException($"Session {id} not found");

        if (session.EndedAt is not null)
            throw new InvalidOperationException($"Session {id} is already ended");

        session.EndedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return session;
    }

    public async Task<Session> UpdateAsync(Guid id, string? name = null, string? notes = null)
    {
        var session = await _db.Sessions.FindAsync(id)
            ?? throw new KeyNotFoundException($"Session {id} not found");

        if (name is not null) session.Name = name;
        if (notes is not null) session.Notes = notes;

        await _db.SaveChangesAsync();
        return session;
    }

    public async Task DeleteAsync(Guid id)
    {
        var session = await _db.Sessions.FindAsync(id)
            ?? throw new KeyNotFoundException($"Session {id} not found");

        _db.Sessions.Remove(session);
        await _db.SaveChangesAsync();
    }
}
