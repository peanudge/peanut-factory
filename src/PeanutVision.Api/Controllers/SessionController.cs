using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
    private readonly ISessionRepository _repository;

    public SessionController(ISessionRepository repository) => _repository = repository;

    public record CreateSessionRequest(string Name, string? Notes = null);
    public record UpdateSessionRequest(string? Name = null, string? Notes = null);

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Session>>> GetAll([FromQuery] int limit = 50)
        => Ok(await _repository.GetAllAsync(limit));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Session>> GetById(Guid id)
    {
        var session = await _repository.GetByIdAsync(id);
        return session is null ? NotFound(new { error = $"Session {id} not found" }) : Ok(session);
    }

    [HttpGet("active")]
    public async Task<ActionResult<Session>> GetActive()
    {
        var session = await _repository.GetActiveAsync();
        return session is null ? NoContent() : Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<Session>> Create([FromBody] CreateSessionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Session name is required" });

        var session = await _repository.CreateAsync(request.Name, request.Notes);
        return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
    }

    [HttpPost("{id:guid}/end")]
    public async Task<ActionResult<Session>> End(Guid id)
    {
        var session = await _repository.EndSessionAsync(id);
        return Ok(session);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Session>> Update(Guid id, [FromBody] UpdateSessionRequest request)
    {
        var session = await _repository.UpdateAsync(id, request.Name, request.Notes);
        return Ok(session);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
