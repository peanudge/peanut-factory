using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PeanutVision.Api.Middleware;
using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.Capture;
using PeanutVision.FakeCamDriver;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

var builder = WebApplication.CreateBuilder(args);

// Configure cam file directory from appsettings.json (relative to project root)
var camFileDir = builder.Configuration["CamFileDirectory"]
    ?? "CamFiles";
var camFilePath = Path.IsPathRooted(camFileDir)
    ? camFileDir
    : Path.Combine(builder.Environment.ContentRootPath, camFileDir);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
    });
builder.Services.AddOpenApi();

builder.Services.AddCamFileService(camFilePath);

var useMock = builder.Configuration.GetValue<bool>("UseMockHardware");

if (useMock)
{
    builder.Services.AddFakeGrabService(config =>
    {
        builder.Configuration.GetSection("FakeHal").Bind(config);
    });
}
else
{
    builder.Services.AddGrabService(autoInitialize: true);
}

var saveSettingsPath = Path.Combine(builder.Environment.ContentRootPath, "image-save-settings.json");
builder.Services.AddSingleton<IImageSaveSettingsService>(new ImageSaveSettingsService(saveSettingsPath));
builder.Services.AddSingleton<IFrameWriter>(_ => new PeanutVision.Capture.ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter()));

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "peanut-vision.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<ICapturedImageRepository, CapturedImageRepository>();
builder.Services.AddSingleton<IThumbnailService, ThumbnailService>();

var presetsPath = Path.Combine(builder.Environment.ContentRootPath, "acquisition-presets.json");
builder.Services.AddSingleton<IAcquisitionPresetService>(new AcquisitionPresetService(presetsPath));

builder.Services.Configure<LatencyRepositoryOptions>(
    builder.Configuration.GetSection("LatencyRepository"));
builder.Services.AddSingleton<ILatencyRepository, LatencyRepository>();
builder.Services.AddSingleton<ILatencyService, LatencyService>();

builder.Services.AddScoped<ISnapshotCapture, SnapshotCapture>();
builder.Services.AddScoped<FrameSavedHandler>();
builder.Services.AddScoped<IAutoSaveService, AutoSaveService>();

// Multi-camera actor system
builder.Services.AddSingleton<CameraRegistry>(sp =>
{
    var registry = new CameraRegistry();
    registry.Register(new CameraActor(
        cameraId:        "cam-1",
        grabService:     sp.GetRequiredService<IGrabService>(),
        camFileService:  sp.GetRequiredService<ICamFileService>(),
        latencyService:  sp.GetRequiredService<ILatencyService>(),
        scopeFactory:    sp.GetRequiredService<IServiceScopeFactory>(),
        frameWriter:     sp.GetRequiredService<IFrameWriter>(),
        saveSettings:    sp.GetRequiredService<IImageSaveSettingsService>(),
        contentRootPath: builder.Environment.ContentRootPath,
        logger:          sp.GetRequiredService<ILogger<CameraActor>>()));
    return registry;
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175",
                           "http://localhost:5176", "http://localhost:5177", "http://localhost:5178",
                           "http://localhost:5179", "http://localhost:5180", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("X-Image-Path");
    });
});

var app = builder.Build();

if (useMock)
{
    app.Logger.LogWarning("FakeCamDriver enabled — using test pattern generator (no hardware)");
}

// Ensure SQLite database and schema are created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    // Add CapturedImages table for existing databases (EnsureCreated is a no-op on existing DBs)
    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS CapturedImages (
            Id TEXT NOT NULL PRIMARY KEY,
            FilePath TEXT NOT NULL,
            ThumbnailPath TEXT,
            Width INTEGER NOT NULL DEFAULT 0,
            Height INTEGER NOT NULL DEFAULT 0,
            FileSizeBytes INTEGER NOT NULL DEFAULT 0,
            Format TEXT NOT NULL DEFAULT '',
            CapturedAt TEXT NOT NULL,
            Tags TEXT NOT NULL DEFAULT '[]',
            Notes TEXT NOT NULL DEFAULT ''
        );
        CREATE INDEX IF NOT EXISTS IX_CapturedImages_CapturedAt ON CapturedImages(CapturedAt);
        """);
    // Add annotation columns to existing databases (no-op if already present)
    try { db.Database.ExecuteSqlRaw("ALTER TABLE CapturedImages ADD COLUMN Tags TEXT NOT NULL DEFAULT '[]'"); } catch { }
    try { db.Database.ExecuteSqlRaw("ALTER TABLE CapturedImages ADD COLUMN Notes TEXT NOT NULL DEFAULT ''"); } catch { }
}

app.UseCors();

app.UseExceptionHandler(o => { });

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "PeanutVision API");
});

app.MapControllers();

app.Run();

public partial class Program { }
