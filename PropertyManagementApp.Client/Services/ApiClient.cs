using System.Net.Http.Json;

namespace PropertyManagementApp.Client.Services;

// typed HttpClient wrapper centralizing error handling and deserialization
public class ApiClient
{
    private readonly HttpClient _http;
    public ApiClient(HttpClient http) => _http = http;

    public async Task<T?> GetAsync<T>(string url)
    {
        var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string url, object body)
    {
        var resp = await _http.PostAsJsonAsync(url, body);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<T>();
    }

    public async Task<HttpResponseMessage> PostAsync(string url, object? body)
    {
        return body == null
            ? await _http.PostAsync(url, null)
            : await _http.PostAsJsonAsync(url, body);
    }

    public async Task<T?> PutAsync<T>(string url, object body)
    {
        var resp = await _http.PutAsJsonAsync(url, body);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<T>();
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await _http.DeleteAsync(url);
    }
}
