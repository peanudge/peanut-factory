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
            Assert.Equal(SaveImageFormat.Png, settings.Format);
            Assert.Equal("capture", settings.FilenamePrefix);
            Assert.Equal("yyyyMMdd_HHmmss_fff", settings.TimestampFormat);
            Assert.False(settings.IncludeSequenceNumber);
            Assert.Equal(SubfolderStrategy.None, settings.SubfolderStrategy);
            Assert.True(settings.AutoSave);
        }
    }

    public class Given_existing_valid_file : ImageSaveSettingsServiceTests
    {
        [Fact]
        public void Then_loads_settings_from_file()
        {
            var expected = new ImageSaveSettings
            {
                OutputDirectory = "/custom/path",
                Format = SaveImageFormat.Bmp,
                FilenamePrefix = "test",
                TimestampFormat = "HHmmss",
                IncludeSequenceNumber = true,
                SubfolderStrategy = SubfolderStrategy.ByDate,
                AutoSave = false,
            };
            File.WriteAllText(_filePath, JsonSerializer.Serialize(expected));

            var service = new ImageSaveSettingsService(_filePath);
            var settings = service.GetSettings();

            Assert.Equal("/custom/path", settings.OutputDirectory);
            Assert.Equal(SaveImageFormat.Bmp, settings.Format);
            Assert.Equal("test", settings.FilenamePrefix);
            Assert.Equal("HHmmss", settings.TimestampFormat);
            Assert.True(settings.IncludeSequenceNumber);
            Assert.Equal(SubfolderStrategy.ByDate, settings.SubfolderStrategy);
            Assert.False(settings.AutoSave);
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
            Assert.Equal(SaveImageFormat.Png, settings.Format);
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
            var newSettings = new ImageSaveSettings
            {
                OutputDirectory = "/saved/path",
                Format = SaveImageFormat.Raw,
                FilenamePrefix = "saved",
                TimestampFormat = "yyyyMMdd",
                IncludeSequenceNumber = true,
                SubfolderStrategy = SubfolderStrategy.ByProfile,
                AutoSave = false,
            };

            await service.SaveSettingsAsync(newSettings);

            Assert.True(File.Exists(_filePath));
            var json = File.ReadAllText(_filePath);
            var loaded = JsonSerializer.Deserialize<ImageSaveSettings>(json);
            Assert.NotNull(loaded);
            Assert.Equal("/saved/path", loaded.OutputDirectory);
            Assert.Equal(SaveImageFormat.Raw, loaded.Format);
        }

        [Fact]
        public async Task Then_in_memory_settings_updated()
        {
            var service = new ImageSaveSettingsService(_filePath);
            var newSettings = new ImageSaveSettings { FilenamePrefix = "updated" };

            await service.SaveSettingsAsync(newSettings);

            Assert.Equal("updated", service.GetSettings().FilenamePrefix);
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
            await service1.SaveSettingsAsync(new ImageSaveSettings { FilenamePrefix = "roundtrip" });

            var service2 = new ImageSaveSettingsService(_filePath);
            Assert.Equal("roundtrip", service2.GetSettings().FilenamePrefix);
        }
    }

    public class When_saving_concurrently : ImageSaveSettingsServiceTests
    {
        [Fact]
        public async Task Then_no_exceptions_thrown()
        {
            var service = new ImageSaveSettingsService(_filePath);
            var tasks = Enumerable.Range(0, 10).Select(i =>
                service.SaveSettingsAsync(new ImageSaveSettings { FilenamePrefix = $"concurrent-{i}" }));

            await Task.WhenAll(tasks);

            // Verify final state is one of the saved values
            var settings = service.GetSettings();
            Assert.StartsWith("concurrent-", settings.FilenamePrefix);
        }
    }

    public class Given_partial_json_schema : ImageSaveSettingsServiceTests
    {
        [Fact]
        public void Then_missing_fields_get_defaults()
        {
            // Write JSON with only some fields
            File.WriteAllText(_filePath, """{"FilenamePrefix":"partial"}""");

            var service = new ImageSaveSettingsService(_filePath);
            var settings = service.GetSettings();

            Assert.Equal("partial", settings.FilenamePrefix);
            // Missing fields should get their default values
            Assert.Equal("CapturedImages", settings.OutputDirectory);
            Assert.Equal(SaveImageFormat.Png, settings.Format);
            Assert.True(settings.AutoSave);
        }
    }
}
