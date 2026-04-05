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

// 사용자 데이터(DB, 설정 파일)는 %APPDATA%\PeanutVision\ 에 저장한다.
// C:\Program Files\ 는 Windows에서 쓰기가 차단되므로 install 경로에 직접 쓰면 크래시가 발생한다.
// %APPDATA% = C:\Users\{사용자}\AppData\Roaming  (항상 쓰기 가능)
var appDataDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "PeanutVision");
Directory.CreateDirectory(appDataDir);  // 첫 실행 시 폴더 자동 생성

var saveSettingsPath = Path.Combine(appDataDir, "image-save-settings.json");
// Use the default OutputDirectory from appsettings.json if no saved settings file exists yet.
var defaultOutputDir = builder.Configuration["ImageSave:OutputDirectory"] ?? "CapturedImages";
builder.Services.AddSingleton<IImageSaveSettingsService>(
    new ImageSaveSettingsService(saveSettingsPath, new ImageSaveSettings { OutputDirectory = defaultOutputDir }));
builder.Services.AddSingleton<IFrameWriter>(_ => new PeanutVision.Capture.ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter()));

var dbPath = Path.Combine(appDataDir, "peanut-vision.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<ICapturedImageRepository, CapturedImageRepository>();
builder.Services.AddSingleton<IThumbnailService, ThumbnailService>();

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

// When running inside Electron, the PEANUT_PORT env var tells us which port to use.
// Electron finds a free port and passes it so there are no port conflicts.
// In development (dotnet run), this env var is not set and ASP.NET uses its default (from launchSettings.json).
var electronPort = Environment.GetEnvironmentVariable("PEANUT_PORT");
if (!string.IsNullOrEmpty(electronPort) && int.TryParse(electronPort, out var parsedPort))
{
    builder.WebHost.UseUrls($"http://localhost:{parsedPort}");
}

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

// Serve React build output from wwwroot/.
// UseStaticFiles serves files like /assets/index.js, /favicon.ico directly.
app.UseDefaultFiles();   // serves index.html for "/"
app.UseStaticFiles();

app.MapGet("/health", () => Results.Ok("healthy"));

// Electron이 앱 종료 전 이 엔드포인트를 호출해 ASP.NET Core가 정상적으로 종료되도록 한다.
// MultiCam 드라이버(McDelete, McCloseDriver)의 정상 종료가 보장된다.
app.MapPost("/shutdown", (IHostApplicationLifetime lifetime) =>
{
    lifetime.StopApplication();
    return Results.Ok();
});

app.MapControllers();

// SPA fallback: for any route that doesn't match a file or API controller,
// return index.html so React Router can handle client-side routing.
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
