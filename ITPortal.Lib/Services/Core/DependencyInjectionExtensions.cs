using Microsoft.Extensions.DependencyInjection;

namespace ITPortal.Lib.Services.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddGitHubServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IHttpClientService, HttpClientService>("GitHubApi");
        serviceCollection.AddSingleton<IGitHubService, GitHubService>();

        return serviceCollection;
    }
}
