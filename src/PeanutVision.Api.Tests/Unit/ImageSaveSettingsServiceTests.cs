using System.Text.Json;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Tests.Unit;

public class ImageSaveSettingsServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _filePath;

    public ImageSaveSettingsServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"pv-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _filePath = Path.Combine(_tempDir, "settings.json");
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    public class Given_no_existing_file : ImageSaveSettingsServiceTests
    {
        [Fact]
        public void Then_returns_default_settings()
        {
            var service = new ImageSaveSettingsService(_filePath);
            var settings = service.GetSettings();

            Assert.Equal("CapturedImages", settings.OutputDirectory);
        }

        [Fact]
        public void Then_uses_provided_defaults()
        {
            var defaults = new ImageSaveSettings { OutputDirectory = "/custom/default" };
            var service = new ImageSaveSettingsService(_filePath, defaults);
            var settings = service.GetSettings();

            Assert.Equal("/custom/default", settings.OutputDirectory);
        }
    }

    public class Given_existing_valid_file : ImageSaveSettingsServiceTests
    {
        [Fact]
        public void Then_loads_settings_from_file()
        {
            var expected = new ImageSaveSettings { OutputDirectory = "/custom/path" };
            File.WriteAllText(_filePath, JsonSerializer.Serialize(expected));

            var service = new ImageSaveSettingsService(_filePath);
            var settings = service.GetSettings();

            Assert.Equal("/custom/path", settings.OutputDirectory);
        }
    }

    public class Given_corrupt_file : ImageSaveSettingsServiceTests
    {
        [Fact]
        public void Then_falls_back_to_defaults()
        {
            File.WriteAllText(_filePath, "{ invalid json !!!");

            var service = new ImageSaveSettingsService(_filePath);
            var settings = service.GetSettings();

            Assert.Equal("CapturedImages", settings.OutputDirectory);
        }
    }

    public class Given_empty_file : ImageSaveSettingsServiceTests
    {
        [Fact]
        public void Then_falls_back_to_defaults()
        {
            File.WriteAllText(_filePath, "");

            var service = new ImageSaveSettingsService(_filePath);
            var settings = service.GetSettings();

            Assert.Equal("CapturedImages", settings.OutputDirectory);
        }
    }

    public class When_saving_settings : ImageSaveSettingsServiceTests
    {
        [Fact]
        public async Task Then_persists_to_file()
        {
            var service = new ImageSaveSettingsService(_filePath);
            var newSettings = new ImageSaveSettings { OutputDirectory = "/saved/path" };

            await service.SaveSettingsAsync(newSettings);

            Assert.True(File.Exists(_filePath));
            var json = File.ReadAllText(_filePath);
            var loaded = JsonSerializer.Deserialize<ImageSaveSettings>(json);
            Assert.NotNull(loaded);
            Assert.Equal("/saved/path", loaded.OutputDirectory);
        }

        [Fact]
        public async Task Then_in_memory_settings_updated()
        {
            var service = new ImageSaveSettingsService(_filePath);
            var newSettings = new ImageSaveSettings { OutputDirectory = "/updated/path" };

            await service.SaveSettingsAsync(newSettings);

            Assert.Equal("/updated/path", service.GetSettings().OutputDirectory);
        }

        [Fact]
        public async Task Then_file_is_indented_json()
        {
            var service = new ImageSaveSettingsService(_filePath);
            await service.SaveSettingsAsync(new ImageSaveSettings());

            var json = File.ReadAllText(_filePath);
            Assert.Contains("\n", json);
            Assert.Contains("  ", json);
        }
    }

    public class When_reloading_after_save : ImageSaveSettingsServiceTests
    {
        [Fact]
        public async Task Then_new_instance_reads_saved_settings()
        {
            var service1 = new ImageSaveSettingsService(_filePath);
            await service1.SaveSettingsAsync(new ImageSaveSettings { OutputDirectory = "/roundtrip" });

            var service2 = new ImageSaveSettingsService(_filePath);
            Assert.Equal("/roundtrip", service2.GetSettings().OutputDirectory);
        }
    }

    public class When_saving_concurrently : ImageSaveSettingsServiceTests
    {
        [Fact]
        public async Task Then_no_exceptions_thrown()
        {
            var service = new ImageSaveSettingsService(_filePath);
            var tasks = Enumerable.Range(0, 10).Select(i =>
                service.SaveSettingsAsync(new ImageSaveSettings { OutputDirectory = $"/concurrent/{i}" }));

            await Task.WhenAll(tasks);

            // Verify final state is one of the saved values
            var settings = service.GetSettings();
            Assert.StartsWith("/concurrent/", settings.OutputDirectory);
        }
    }
}
