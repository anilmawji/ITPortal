using ScriptProfiler.Lib.Automation.Job;
using ScriptProfiler.Lib.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ScriptProfiler.Lib.Services;

public static class DependencyInjectionExtensions
{
    private static Dictionary<string, bool> OptionReferences = new();
    private static bool AuthorizationCoreAdded;

    public static void AddOption<T>(this IServiceCollection collection, IConfiguration configuration) where T : class
    {
        collection.Configure<T>(configuration.GetSection(typeof(T).Name));
    }

    public static void TryAddOption<T>(this IServiceCollection collection, IConfiguration configuration) where T : class
    {
        string sectionName = typeof(T).Name;

        if (OptionReferences.ContainsKey(sectionName)) return;
        OptionReferences[sectionName] = true;

        collection.Configure<T>(configuration.GetSection(sectionName));
    }

    private static void TryAddAuthorizationCore(this IServiceCollection collection)
    {
        if (AuthorizationCoreAdded) return;
        AuthorizationCoreAdded = true;

        collection.AddAuthorizationCore();
    }

    public static IServiceCollection AddMsalAuthenticationServices(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.TryAddAuthorizationCore();
        collection.TryAddOption<AzureAdSettings>(configuration);
        collection.TryAddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();
        collection.TryAddSingleton<IAccessTokenProvider, TokenProvider>();
        collection.AddSingleton<IAuthenticationService, MsalAuthenticationService>();

        return collection;
    }

    public static IServiceCollection AddScriptJobServices(this IServiceCollection collection, IConfiguration configuration, string saveFolderPath)
    {
        collection.AddSingleton<IScriptJobService, ScriptJobService>();
        collection.AddOption<SerializationSettings>(configuration);

        collection.AddSingleton<ISerializationService<ScriptJob>, JsonSerializationService<ScriptJob>>(
            serviceProvider => new JsonSerializationService<ScriptJob>(
                options: serviceProvider.GetRequiredService<IOptions<SerializationSettings>>(),
                typeInfo: ScriptJobContext.Default.ScriptJob,
                saveFolderPath));

        collection.AddSingleton<ISerializationService<ScriptJobResult>, JsonSerializationService<ScriptJobResult>>(
            serviceProvider => new JsonSerializationService<ScriptJobResult>(
                options: serviceProvider.GetRequiredService<IOptions<SerializationSettings>>(),
                typeInfo: ScriptJobResultContext.Default.ScriptJobResult,
                saveFolderPath));

        return collection;
    }

    public static IServiceCollection AddGitHubServices(this IServiceCollection collection)
    {
        collection.AddHttpClient<IHttpClientService, HttpClientService>("GitHubApi");
        collection.AddSingleton<IGitHubService, GitHubService>();

        return collection;
    }
}
