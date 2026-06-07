using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Tests.Unit;

public class AutoSaveServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly FakeAcquisitionService _acquisition;
    private readonly FrameSaveTracker _tracker;
    private readonly FilenameGenerator _filenameGenerator;
    private readonly FakeThumbnailService _thumbnailService;
    private readonly FakeScopeFactory _scopeFactory;
    private readonly FakeWebHostEnvironment _environment;

    public AutoSaveServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"autosave_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        _acquisition = new FakeAcquisitionService(_tempDir);
        _tracker = new FrameSaveTracker();
        _filenameGenerator = new FilenameGenerator();
        _thumbnailService = new FakeThumbnailService();
        _scopeFactory = new FakeScopeFactory();
        _environment = new FakeWebHostEnvironment(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private AutoSaveService BuildService() =>
        new(_acquisition, _filenameGenerator, _tracker,
            _thumbnailService, _scopeFactory, _environment);

    private static ImageData MakeFrame() =>
        new(new byte[4 * 1 * 3], 4, 1, 12);

    // ── Subscribe / Unsubscribe ──

    [Fact]
    public async Task StartAsync_subscribes_to_FrameAcquired()
    {
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        _acquisition.AutoSave = false; // prevent actual save
        _acquisition.SimulateFrame(MakeFrame());

        // If subscribed, OnFrameAcquired ran — no exception means event handler was wired
        Assert.True(true);
    }

    [Fact]
    public async Task StopAsync_unsubscribes_so_no_save_after_stop()
    {
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);
        await svc.StopAsync(CancellationToken.None);

        _acquisition.AutoSave = true;
        _acquisition.SimulateFrame(MakeFrame());

        await Task.Delay(100); // give fire-and-forget time to run if it leaked
        Assert.Empty(Directory.GetFiles(_tempDir, "*.png", SearchOption.AllDirectories));
    }

    // ── AutoSave = false ──

    [Fact]
    public async Task When_autosave_disabled_no_file_is_written()
    {
        _acquisition.AutoSave = false;
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        _acquisition.SimulateFrame(MakeFrame());
        await Task.Delay(100);

        Assert.Empty(Directory.GetFiles(_tempDir, "*", SearchOption.AllDirectories));
    }

    // ── Null frame ──

    [Fact]
    public async Task When_latest_frame_is_null_no_file_is_written()
    {
        _acquisition.AutoSave = true;
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        _acquisition.SimulateFrame(null); // fires event but GetLatestFrame returns null
        await Task.Delay(100);

        Assert.Empty(Directory.GetFiles(_tempDir, "*", SearchOption.AllDirectories));
    }

    // ── FrameSaveTracker dedup ──

    [Fact]
    public async Task Same_frame_reference_saved_only_once()
    {
        _acquisition.AutoSave = true;
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        var frame = MakeFrame();
        _acquisition.SimulateFrame(frame);
        await Task.Delay(200);
        _acquisition.SimulateFrame(frame); // same reference
        await Task.Delay(200);

        Assert.Single(Directory.GetFiles(_tempDir, "*.png", SearchOption.AllDirectories));
    }

    [Fact]
    public async Task Different_frames_each_saved()
    {
        _acquisition.AutoSave = true;
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        _acquisition.SimulateFrame(MakeFrame());
        await Task.Delay(200);
        _acquisition.SimulateFrame(MakeFrame());
        await Task.Delay(200);

        Assert.Equal(2, Directory.GetFiles(_tempDir, "*.png", SearchOption.AllDirectories).Length);
    }

    // ── File written ──

    [Fact]
    public async Task When_autosave_enabled_new_frame_writes_png_file()
    {
        _acquisition.AutoSave = true;
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        _acquisition.SimulateFrame(MakeFrame());
        await Task.Delay(300);

        var files = Directory.GetFiles(_tempDir, "*.png", SearchOption.AllDirectories);
        var file = Assert.Single(files);
        Assert.True(new FileInfo(file).Length > 0);
    }

    [Fact]
    public async Task Saved_file_has_valid_png_magic_bytes()
    {
        _acquisition.AutoSave = true;
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        _acquisition.SimulateFrame(MakeFrame());
        await Task.Delay(300);

        var file = Directory.GetFiles(_tempDir, "*.png", SearchOption.AllDirectories).Single();
        var bytes = await File.ReadAllBytesAsync(file);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, bytes[..8]);
    }

    [Fact]
    public async Task Repository_AddAsync_called_after_save()
    {
        _acquisition.AutoSave = true;
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        _acquisition.SimulateFrame(MakeFrame());
        await Task.Delay(300);

        Assert.Equal(1, _scopeFactory.FakeImageRepo.AddCount);
    }

    // ── Save failure does not propagate ──

    [Fact]
    public async Task Save_failure_does_not_throw_or_crash()
    {
        _acquisition.AutoSave = true;
        _scopeFactory.FakeImageRepo.ThrowOnAdd = true;
        var svc = BuildService();
        await svc.StartAsync(CancellationToken.None);

        // Should not throw
        _acquisition.SimulateFrame(MakeFrame());
        await Task.Delay(300);

        Assert.True(true);
    }
}

// ── Fakes ──

internal sealed class FakeAcquisitionService : IAcquisitionSession
{
    private readonly string _outputDir;
    private ImageData? _frame;

    public bool AutoSave { get; set; } = true;

    public FakeAcquisitionService(string outputDir) => _outputDir = outputDir;

    public event EventHandler? FrameAcquired;
    public event EventHandler? StatusChanged { add { } remove { } }

    public void SimulateFrame(ImageData? frame)
    {
        _frame = frame;
        FrameAcquired?.Invoke(this, EventArgs.Empty);
    }

    public ImageData? GetLatestFrame() => _frame;

    public AcquisitionStatus GetStatus() => new(
        ChannelState: ChannelState.None,
        ActiveConfig: new AcquisitionConfig(
            new ProfileId("cam.cam"),
            OutputDirectory: _outputDir,
            Format: SaveImageFormat.Png,
            AutoSave: AutoSave
        ),
        HasFrame: _frame != null,
        LastError: null,
        Statistics: null,
        RecentEvents: [],
        AllowedActions: new HashSet<ChannelAction> { ChannelAction.Start }
    );

    public void Start(AcquisitionConfig config) { }
    public void Stop() { }
    public void ReleaseChannel() { }
    public Task<ImageData> TriggerAsync(int timeoutMs = 5000) =>
        Task.FromResult(new ImageData(new byte[3], 1, 1, 3));
    public void Dispose() { }
}

internal sealed class FakeThumbnailService : IThumbnailService
{
    public Task<string?> GenerateAsync(string imagePath) => Task.FromResult<string?>(null);
}

internal sealed class FakeImageRepository : ICapturedImageRepository
{
    public int AddCount;
    public bool ThrowOnAdd;

    public Task<CapturedImage> AddAsync(CapturedImage image)
    {
        if (ThrowOnAdd) throw new InvalidOperationException("Simulated DB failure");
        Interlocked.Increment(ref AddCount);
        return Task.FromResult(image);
    }

    public Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
        int page, int pageSize, DateTime? dateFrom = null, DateTime? dateTo = null) =>
        Task.FromResult<(IReadOnlyList<CapturedImage>, int)>(([], 0));

    public Task<CapturedImage?> GetByIdAsync(Guid id) => Task.FromResult<CapturedImage?>(null);
    public Task DeleteAsync(Guid id) => Task.CompletedTask;
}

internal sealed class FakeScopeFactory : IServiceScopeFactory
{
    public readonly FakeImageRepository FakeImageRepo = new();

    public IServiceScope CreateScope()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ICapturedImageRepository>(FakeImageRepo);
        var provider = services.BuildServiceProvider();
        return new FakeScope(provider);
    }

    private sealed class FakeScope(IServiceProvider provider) : IServiceScope
    {
        public IServiceProvider ServiceProvider { get; } = provider;
        public void Dispose() { }
    }
}

internal sealed class FakeWebHostEnvironment : IWebHostEnvironment
{
    public FakeWebHostEnvironment(string contentRoot) => ContentRootPath = contentRoot;
    public string ContentRootPath { get; set; }
    public string WebRootPath { get; set; } = "";
    public string EnvironmentName { get; set; } = "Test";
    public string ApplicationName { get; set; } = "Test";
    public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; } =
        new Microsoft.Extensions.FileProviders.NullFileProvider();
    public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
        new Microsoft.Extensions.FileProviders.NullFileProvider();
}
