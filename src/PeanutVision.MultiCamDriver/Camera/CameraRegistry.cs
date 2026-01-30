namespace PeanutVision.MultiCamDriver.Camera;

/// <summary>
/// Registry of known camera profiles. Supports the Open/Closed Principle
/// by allowing new profiles to be registered without modifying existing code.
/// </summary>
public sealed class CameraRegistry
{
    private readonly Dictionary<string, CameraProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the default registry instance with built-in camera profiles.
    /// </summary>
    public static CameraRegistry Default { get; } = CreateDefault();

    /// <summary>
    /// Gets all registered camera profiles.
    /// </summary>
    public IReadOnlyCollection<CameraProfile> Profiles => _profiles.Values;

    /// <summary>
    /// Gets all registered profile IDs.
    /// </summary>
    public IReadOnlyCollection<string> ProfileIds => _profiles.Keys;

    /// <summary>
    /// Registers a camera profile.
    /// </summary>
    public CameraRegistry Register(CameraProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        _profiles[profile.Id] = profile;
        return this;
    }

    /// <summary>
    /// Unregisters a camera profile by ID.
    /// </summary>
    public CameraRegistry Unregister(string profileId)
    {
        _profiles.Remove(profileId);
        return this;
    }

    /// <summary>
    /// Gets a camera profile by ID.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If profile is not found</exception>
    public CameraProfile GetProfile(string profileId)
    {
        if (_profiles.TryGetValue(profileId, out var profile))
        {
            return profile;
        }

        var available = string.Join(", ", _profiles.Keys);
        throw new KeyNotFoundException(
            $"Camera profile '{profileId}' not found. Available: {available}");
    }

    /// <summary>
    /// Tries to get a camera profile by ID.
    /// </summary>
    public bool TryGetProfile(string profileId, out CameraProfile? profile)
    {
        return _profiles.TryGetValue(profileId, out profile);
    }

    /// <summary>
    /// Checks if a profile is registered.
    /// </summary>
    public bool HasProfile(string profileId)
    {
        return _profiles.ContainsKey(profileId);
    }

    /// <summary>
    /// Gets profiles by manufacturer.
    /// </summary>
    public IEnumerable<CameraProfile> GetByManufacturer(string manufacturer)
    {
        return _profiles.Values.Where(p =>
            p.Manufacturer.Equals(manufacturer, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets profiles by model.
    /// </summary>
    public IEnumerable<CameraProfile> GetByModel(string model)
    {
        return _profiles.Values.Where(p =>
            p.Model.Equals(model, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the default camera profile (first registered, or null if empty).
    /// </summary>
    public CameraProfile? DefaultProfile => _profiles.Values.FirstOrDefault();

    private static CameraRegistry CreateDefault()
    {
        var registry = new CameraRegistry();

        // Register built-in Crevis profiles
        foreach (var profile in CrevisProfiles.All)
        {
            registry.Register(profile);
        }

        return registry;
    }
}
