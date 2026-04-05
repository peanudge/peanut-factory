using System.Collections.Concurrent;

namespace PeanutVision.Api.Services.Camera;

/// <summary>
/// Singleton registry mapping camera IDs to CameraActors.
/// Thread-safe via ConcurrentDictionary; contains no mutable state of its own.
/// </summary>
public sealed class CameraRegistry
{
    private readonly ConcurrentDictionary<string, ICameraActor> _actors = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Registers an actor. Throws if a camera with the same ID already exists.</summary>
    public void Register(ICameraActor actor)
    {
        if (!_actors.TryAdd(actor.CameraId, actor))
            throw new InvalidOperationException($"Camera '{actor.CameraId}' is already registered.");
    }

    /// <summary>Returns the actor for <paramref name="cameraId"/>, or null if not found.</summary>
    public ICameraActor? TryGet(string cameraId)
        => _actors.TryGetValue(cameraId, out var actor) ? actor : null;

    /// <summary>Returns all registered camera IDs.</summary>
    public IReadOnlyList<string> GetAllIds()
        => _actors.Keys.ToList();
}
