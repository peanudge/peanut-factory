using System.Text.Json;

namespace PeanutVision.Api.Tests.Infrastructure;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<JsonDocument> ReadJsonDocumentAsync(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    public static async Task<HttpResponseMessage> PostJsonAsync(
        this HttpClient client, string url, object payload)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return await client.PostAsync(url, content);
    }

    public static async Task<HttpResponseMessage> PutJsonAsync(
        this HttpClient client, string url, object payload)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return await client.PutAsync(url, content);
    }
}
