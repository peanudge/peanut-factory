using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraCaptureFlowSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraCaptureFlowSpec(PeanutVisionApiFactory factory) { _factory = factory; _client = factory.CreateClient(); }
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task snapshot_creates_image_record_in_catalog()
    {
        var before = await GetTotalImageCount();

        var snapResponse = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, snapResponse.StatusCode);

        var after = await GetTotalImageCount();
        Assert.True(after > before, $"Expected image count to increase from {before} but got {after}");
    }

    [Fact]
    public async Task trigger_and_wait_saves_image_when_autosave_enabled()
    {
        var countBefore = await GetTotalImageCount();

        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        var triggerResponse = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.OK, triggerResponse.StatusCode);

        var countAfter = await GetTotalImageCount();
        Assert.True(countAfter > countBefore, "Trigger with auto-save should create a new image record");

        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var listDoc = await listResponse.ReadJsonDocumentAsync();
        var latest = listDoc.RootElement.GetProperty("items").EnumerateArray().First();
        Assert.False(string.IsNullOrEmpty(latest.GetProperty("filePath").GetString()));
    }

    [Fact]
    public async Task captured_image_has_correct_metadata()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, (await _client.PostAsync($"/api/cameras/{CamId}/trigger", null)).StatusCode);

        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var doc = await listResponse.ReadJsonDocumentAsync();
        var item = doc.RootElement.GetProperty("items").EnumerateArray().First();

        Assert.True(Guid.TryParse(item.GetProperty("id").GetString(), out var id) && id != Guid.Empty);
        _ = item.GetProperty("capturedAt").GetDateTime();
        Assert.True(item.GetProperty("width").GetInt32() > 0);
        Assert.True(item.GetProperty("height").GetInt32() > 0);
        Assert.True(item.GetProperty("fileSizeBytes").GetInt64() > 0);
        Assert.False(string.IsNullOrEmpty(item.GetProperty("format").GetString()));
    }

    [Fact]
    public async Task captured_image_file_is_accessible()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);

        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var doc = await listResponse.ReadJsonDocumentAsync();
        var idStr = doc.RootElement.GetProperty("items").EnumerateArray().First().GetProperty("id").GetString()!;
        var fileResponse = await _client.GetAsync($"/api/images/{idStr}/file");
        Assert.Equal(HttpStatusCode.OK, fileResponse.StatusCode);
        var bytes = await fileResponse.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 0);
    }

    private async Task<int> GetTotalImageCount()
    {
        var response = await _client.GetAsync("/api/images?pageSize=1");
        response.EnsureSuccessStatusCode();
        using var doc = await response.ReadJsonDocumentAsync();
        return doc.RootElement.GetProperty("totalCount").GetInt32();
    }
}
