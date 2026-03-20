namespace PeanutVision.Api.Services;

public interface ISessionRepository
{
    Task<Session> CreateAsync(string name, string? notes = null);
    Task<Session?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Session>> GetAllAsync(int limit = 50);
    Task<Session?> GetActiveAsync();
    Task<Session> EndSessionAsync(Guid id);
    Task<Session> UpdateAsync(Guid id, string? name = null, string? notes = null);
    Task DeleteAsync(Guid id);
}
