using System.Text.Json;
using HttpHeaders = System.Net.Http.Headers;

namespace ITPortal.Lib.Services;

public class HttpClientService : IHttpClientService
{
    public HttpClient Client { get; }

    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpClientService(HttpClient httpClient)
    {
        Client = httpClient;
        Client.DefaultRequestHeaders.UserAgent.Add(new HttpHeaders.ProductInfoHeaderValue("ITPTest", "1.0"));
        Client.DefaultRequestHeaders.Accept.Add(new HttpHeaders.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<T?> GetModelAsync<T>(string url)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(url));

        Stream streamResponse = await Client.GetStreamAsync(url)
            .ConfigureAwait(false);

        // Convert JSON stream to object with JSON strings
        T? modelResponse = await JsonSerializer.DeserializeAsync<T?>(streamResponse, _options)
            .ConfigureAwait(false);

        return modelResponse;
    }
}
