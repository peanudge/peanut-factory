using System.Text.Json;

namespace PeanutVision.Api.Services;

public interface IImageSaveSettingsService
{
    ImageSaveSettings GetSettings();
    Task SaveSettingsAsync(ImageSaveSettings settings);
}

public sealed class ImageSaveSettingsService : IImageSaveSettingsService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    private readonly string _filePath;
    private readonly ImageSaveSettings _defaults;
    private volatile ImageSaveSettings _settings;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public ImageSaveSettingsService(string filePath, ImageSaveSettings? defaults = null)
    {
        _filePath = filePath;
        _defaults = defaults ?? new ImageSaveSettings();
        _settings = Load();
    }

    public ImageSaveSettings GetSettings() => _settings;

    public async Task SaveSettingsAsync(ImageSaveSettings settings)
    {
        await _writeLock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
            _settings = settings;
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private ImageSaveSettings Load()
    {
        if (!File.Exists(_filePath)) return _defaults;
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<ImageSaveSettings>(json) ?? _defaults;
        }
        catch
        {
            return _defaults;
        }
    }
}
