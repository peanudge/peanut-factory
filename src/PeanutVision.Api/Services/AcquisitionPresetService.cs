using System.Text.Json;

namespace PeanutVision.Api.Services;

public interface IAcquisitionPresetService
{
    IReadOnlyList<AcquisitionPreset> GetAll();
    AcquisitionPreset? GetByName(string name);
    Task SaveAsync(AcquisitionPreset preset);
    Task DeleteAsync(string name);
}

public sealed class AcquisitionPresetService : IAcquisitionPresetService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<AcquisitionPreset> _presets;

    public AcquisitionPresetService(string filePath)
    {
        _filePath = filePath;
        _presets = Load();
    }

    public IReadOnlyList<AcquisitionPreset> GetAll() => _presets.AsReadOnly();

    public AcquisitionPreset? GetByName(string name)
        => _presets.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public async Task SaveAsync(AcquisitionPreset preset)
    {
        await _lock.WaitAsync();
        try
        {
            var index = _presets.FindIndex(p => p.Name.Equals(preset.Name, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
                _presets[index] = preset;
            else
                _presets.Add(preset);

            await PersistAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DeleteAsync(string name)
    {
        await _lock.WaitAsync();
        try
        {
            var removed = _presets.RemoveAll(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (removed == 0)
                throw new KeyNotFoundException($"Preset '{name}' not found");
            await PersistAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task PersistAsync()
    {
        var json = JsonSerializer.Serialize(_presets, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private List<AcquisitionPreset> Load()
    {
        if (!File.Exists(_filePath)) return [];
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<AcquisitionPreset>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
