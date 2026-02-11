using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;

var builder = WebApplication.CreateBuilder(args);

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

app.MapOpenApi();

app.UseCors();

app.MapControllers();

app.Run();

public partial class Program { }
