namespace ITPortal.Lib.Services;

public interface IHttpClientService
{
    public HttpClient Client { get; }

    public Task<T?> GetModelAsync<T>(string url);
}
