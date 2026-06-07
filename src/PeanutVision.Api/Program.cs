using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PeanutVision.Api.Middleware;
using PeanutVision.Api.Services;
using PeanutVision.FakeCamDriver;
using PeanutVision.MultiCamDriver;

System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());

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

builder.Services.AddSingleton<FilenameGenerator>();
builder.Services.AddSingleton<FrameSaveTracker>();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "peanut-vision.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<ICapturedImageRepository, CapturedImageRepository>();


var presetsPath = Path.Combine(builder.Environment.ContentRootPath, "acquisition-presets.json");
builder.Services.AddSingleton<IAcquisitionConfigPresetService>(new AcquisitionConfigPresetService(presetsPath));

builder.Services.Configure<LatencyRepositoryOptions>(
    builder.Configuration.GetSection("LatencyRepository"));
builder.Services.AddSingleton<ILatencyRepository, LatencyRepository>();
builder.Services.AddSingleton<ILatencyService, LatencyService>();

builder.Services.AddSingleton<AcquisitionConfigValidator>();
builder.Services.AddSingleton<AcquisitionManager>();
builder.Services.AddSingleton<IAcquisitionSession>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddHostedService<FrameSaveService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000")
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

// Shutdown phase timing — remove after diagnosis
var shutdownLogger = app.Logger;
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

System.Diagnostics.Stopwatch? shutdownSw = null;
lifetime.ApplicationStopping.Register(() =>
{
    shutdownSw = System.Diagnostics.Stopwatch.StartNew();
    shutdownLogger.LogWarning("[SHUTDOWN] ApplicationStopping fired (+{Elapsed}ms)", shutdownSw.ElapsedMilliseconds);
});
lifetime.ApplicationStopped.Register(() =>
{
    shutdownLogger.LogWarning("[SHUTDOWN] ApplicationStopped fired (+{Elapsed}ms — IHostedService phase done)", shutdownSw?.ElapsedMilliseconds);
});

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
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
