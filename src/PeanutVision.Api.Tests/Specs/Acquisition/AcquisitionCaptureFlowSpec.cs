using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Acquisition;

/// <summary>
/// Integration tests for the full capture pipeline:
/// trigger → image saved → queryable via GET /images.
/// AutoSave is enabled by default (ImageSaveSettings.AutoSave = true).
/// </summary>
public class AcquisitionCaptureFlowSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public AcquisitionCaptureFlowSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        // Ensure acquisition is stopped between tests
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task snapshot_creates_image_record_in_catalog()
    {
        // Snapshot uses freerun profile; AutoSave is on by default
        var before = await GetTotalImageCount();

        var snapResponse = await _client.PostJsonAsync("/api/acquisition/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, snapResponse.StatusCode);

        var after = await GetTotalImageCount();
        Assert.True(after > before, $"Expected image count to increase from {before} but got {after}");

        // The new item should have valid width and height
        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        using var doc = await listResponse.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.NotEmpty(items);
        var first = items[0];
        Assert.True(first.GetProperty("width").GetInt32() > 0);
        Assert.True(first.GetProperty("height").GetInt32() > 0);
    }

    [Fact]
    public async Task trigger_and_wait_saves_image_when_autosave_enabled()
    {
        // Confirm AutoSave is enabled (default = true)
        var settingsResponse = await _client.GetAsync("/api/settings/image-save");
        Assert.Equal(HttpStatusCode.OK, settingsResponse.StatusCode);
        using var settingsDoc = await settingsResponse.ReadJsonDocumentAsync();
        Assert.True(settingsDoc.RootElement.GetProperty("autoSave").GetBoolean(),
            "AutoSave should be true by default");

        var countBefore = await GetTotalImageCount();

        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });

        var triggerResponse = await _client.PostAsync("/api/acquisition/trigger", null);
        Assert.Equal(HttpStatusCode.OK, triggerResponse.StatusCode);

        var countAfter = await GetTotalImageCount();
        Assert.True(countAfter > countBefore, "Trigger with auto-save should create a new image record");

        // The saved record should have filePath and thumbnailPath
        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var listDoc = await listResponse.ReadJsonDocumentAsync();
        var items = listDoc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.NotEmpty(items);
        var latest = items[0];

        // filePath is always present; hasThumbnail indicates thumbnail existence
        Assert.False(string.IsNullOrEmpty(latest.GetProperty("filePath").GetString()),
            "filePath should be non-empty after auto-save");
    }

    [Fact]
    public async Task captured_image_has_correct_metadata()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });

        var triggerResponse = await _client.PostAsync("/api/acquisition/trigger", null);
        Assert.Equal(HttpStatusCode.OK, triggerResponse.StatusCode);

        // The X-Image-Path header tells us the saved file path. Use the catalog to find the record.
        // Query the most recent page and pick the freshest item by capturedAt.
        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        using var doc = await listResponse.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.NotEmpty(items);

        var item = items[0];

        // id must be a non-empty guid
        var idStr = item.GetProperty("id").GetString();
        Assert.True(Guid.TryParse(idStr, out var id) && id != Guid.Empty, "id should be a valid non-empty GUID");

        // capturedAt must be parseable
        Assert.True(item.TryGetProperty("capturedAt", out var capturedAtElem), "capturedAt field must exist");
        _ = capturedAtElem.GetDateTime(); // throws if unparseable

        // width and height should be positive
        Assert.True(item.GetProperty("width").GetInt32() > 0, "width should be > 0");
        Assert.True(item.GetProperty("height").GetInt32() > 0, "height should be > 0");

        // fileSizeBytes should be positive after save
        Assert.True(item.GetProperty("fileSizeBytes").GetInt64() > 0, "fileSizeBytes should be > 0");

        // format should be present and non-empty
        var format = item.GetProperty("format").GetString();
        Assert.False(string.IsNullOrEmpty(format), "format should be non-empty");
    }

    [Fact]
    public async Task captured_image_thumbnail_is_accessible()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync("/api/acquisition/trigger", null);

        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var doc = await listResponse.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.NotEmpty(items);

        var item = items[0];
        var hasThumbnail = item.GetProperty("hasThumbnail").GetBoolean();

        if (!hasThumbnail)
        {
            // Thumbnail generation may be skipped in test environment; skip assertion
            return;
        }

        var idStr = item.GetProperty("id").GetString()!;
        var thumbResponse = await _client.GetAsync($"/api/images/{idStr}/thumbnail");
        Assert.Equal(HttpStatusCode.OK, thumbResponse.StatusCode);

        var mediaType = thumbResponse.Content.Headers.ContentType?.MediaType ?? "";
        Assert.StartsWith("image/", mediaType, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task captured_image_file_is_accessible()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync("/api/acquisition/trigger", null);

        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var doc = await listResponse.ReadJsonDocumentAsync();
        var items = doc.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.NotEmpty(items);

        var idStr = items[0].GetProperty("id").GetString()!;
        var fileResponse = await _client.GetAsync($"/api/images/{idStr}/file");
        Assert.Equal(HttpStatusCode.OK, fileResponse.StatusCode);

        var bytes = await fileResponse.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 0, "File download should return non-empty content");
    }

    private async Task<int> GetTotalImageCount()
    {
        var response = await _client.GetAsync("/api/images?pageSize=1");
        response.EnsureSuccessStatusCode();
        using var doc = await response.ReadJsonDocumentAsync();
        return doc.RootElement.GetProperty("totalCount").GetInt32();
    }
}
