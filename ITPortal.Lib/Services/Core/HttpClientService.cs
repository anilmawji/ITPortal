using System.Text.Json;

namespace ITPortal.Lib.Services.Core;

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
        Client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("ITPTest", "1.0"));
        Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<T?> GetModelAsync<T>(string url)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        Stream streamResponse = await Client.GetStreamAsync(url)
            .ConfigureAwait(false);

        // Convert JSON stream to object with JSON strings
        T? modelResponse = await JsonSerializer.DeserializeAsync<T?>(streamResponse, _options)
            .ConfigureAwait(false);

        return modelResponse;
    }
}
