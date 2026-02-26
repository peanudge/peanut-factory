using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

public class AcquisitionTriggerSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public AcquisitionTriggerSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task Trigger_when_inactive_returns_409()
    {
        var response = await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Trigger_when_active_returns_png_image()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Trigger_response_contains_valid_png_bytes()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PostAsync("/api/acquisition/trigger", null);
        var bytes = await response.Content.ReadAsByteArrayAsync();

        // PNG magic bytes: 137 80 78 71 13 10 26 10
        Assert.True(bytes.Length > 8);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, bytes[..8]);
    }

    [Fact]
    public async Task Trigger_increments_hal_trigger_count()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        _factory.ResetMockState();

        // Triggers are sequential now (each awaits the frame)
        await _client.PostAsync("/api/acquisition/trigger", null);
        await _client.PostAsync("/api/acquisition/trigger", null);

        Assert.Equal(2, _factory.MockHal.CallLog.SoftwareTriggerCount);
    }

    [Fact]
    public async Task Trigger_when_active_saves_image_to_output_directory()
    {
        var outputDir = Path.Combine(Path.GetTempPath(), $"peanut_test_{Guid.NewGuid():N}");
        try
        {
            using var customFactory = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ImageOutputDirectory", outputDir);
            });
            using var client = customFactory.CreateClient();

            await client.PostJsonAsync("/api/acquisition/start",
                new { profileId = "crevis-tc-a160k-freerun-rgb8" });

            var response = await client.PostAsync("/api/acquisition/trigger", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Image-Path"));

            var savedPath = response.Headers.GetValues("X-Image-Path").First();
            Assert.True(File.Exists(savedPath));

            var savedBytes = await File.ReadAllBytesAsync(savedPath);
            Assert.True(savedBytes.Length > 8);
            Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, savedBytes[..8]);

            await client.PostAsync("/api/acquisition/stop", null);
        }
        finally
        {
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, recursive: true);
        }
    }
}
