using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PeanutVision.Api.Middleware;
using PeanutVision.Api.Services;
using PeanutVision.FakeCamDriver;
using PeanutVision.MultiCamDriver;

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
builder.Services.AddSingleton<FilenameGenerator>();
builder.Services.AddSingleton<FrameSaveTracker>();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "peanut-vision.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

var presetsPath = Path.Combine(builder.Environment.ContentRootPath, "acquisition-presets.json");
builder.Services.AddSingleton<IAcquisitionPresetService>(new AcquisitionPresetService(presetsPath));

builder.Services.AddSingleton<AcquisitionManager>();
builder.Services.AddSingleton<IAcquisitionService>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<IChannelCalibration>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<ICalibrationService, CalibrationManager>();

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

// Ensure SQLite database and schema are created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    ExceptionHandler = async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
    }
});

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "PeanutVision API");
});

app.MapControllers();

app.Run();

public partial class Program { }
