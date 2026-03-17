using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;

var builder = WebApplication.CreateBuilder(args);

// Configure cam file directory from appsettings.json (relative to project root)
var camFileDir = builder.Configuration["CamFileDirectory"]
    ?? "CamFiles";
var camFilePath = Path.IsPathRooted(camFileDir)
    ? camFileDir
    : Path.Combine(builder.Environment.ContentRootPath, camFileDir);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCamFileService(camFilePath);
builder.Services.AddGrabService(autoInitialize: true);
builder.Services.AddSingleton<AcquisitionManager>();
builder.Services.AddSingleton<IAcquisitionService>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<ICalibrationService, CalibrationManager>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.UseExceptionHandler(error => error.Run(async context =>
{
    context.Response.StatusCode = 500;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
}));

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "PeanutVision API");
});

app.MapControllers();

app.Run();

public partial class Program { }
