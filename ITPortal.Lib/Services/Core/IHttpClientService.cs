namespace ITPortal.Lib.Services.Core;

public interface IHttpClientService
{
    public HttpClient Client { get; }

    public Task<T?> GetModelAsync<T>(string url);
}
