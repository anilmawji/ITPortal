using ITPortal.Lib.Automation.Job;
using Microsoft.Extensions.DependencyInjection;

namespace ITPortal.Lib.Services;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddGitHubServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IHttpClientService, HttpClientService>("GitHubApi");
        serviceCollection.AddSingleton<IGitHubService, GitHubService>();

        return serviceCollection;
    }

    public static IServiceCollection AddScriptJobServices(this IServiceCollection serviceCollection, string jobFileSaveFolder)
    {
        serviceCollection.AddSingleton<IScriptJobService, ScriptJobService>();

        serviceCollection.AddSingleton<ISerializationService<ScriptJob>>(x =>
            ActivatorUtilities.CreateInstance<JsonSerializationService<ScriptJob>>(x,
                Path.Combine(jobFileSaveFolder, "Jobs"),
                ScriptJobContext.Default.ScriptJob, true));

        serviceCollection.AddSingleton<ISerializationService<ScriptJobResult>>(x =>
            ActivatorUtilities.CreateInstance<JsonSerializationService<ScriptJobResult>>(x,
                Path.Combine(jobFileSaveFolder, "JobResults"),
                ScriptJobResultContext.Default.ScriptJobResult, true));

        return serviceCollection;
    }
}
