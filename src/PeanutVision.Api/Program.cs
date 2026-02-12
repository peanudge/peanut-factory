using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

var builder = WebApplication.CreateBuilder(args);

// Configure cam file directory from appsettings.json (relative to project root)
var camFileDir = builder.Configuration["CamFileDirectory"];
if (!string.IsNullOrEmpty(camFileDir))
{
    var fullPath = Path.IsPathRooted(camFileDir)
        ? camFileDir
        : Path.Combine(builder.Environment.ContentRootPath, camFileDir);
    CamFileResource.SetDirectory(fullPath);
}

// Load camera profiles from cam files in the directory
CameraRegistry.Default.LoadFromDirectory();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddGrabService(autoInitialize: true);
builder.Services.AddSingleton<AcquisitionManager>();

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
