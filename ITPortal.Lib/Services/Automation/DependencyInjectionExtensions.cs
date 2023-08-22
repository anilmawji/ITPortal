using Microsoft.Extensions.DependencyInjection;
using ITPortal.Lib.Services.Automation.Job;
using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation;

public static class DependencyInjectionExtensions
{
    private static bool ScriptJobServiceAdded;

    private static IServiceCollection TryAddScriptJobService(this IServiceCollection serviceCollection)
    {
        if (!ScriptJobServiceAdded)
        {
            serviceCollection.AddSingleton<IScriptJobService, ScriptJobService>();
            ScriptJobServiceAdded = true;
        }
        return serviceCollection;
    }

    public static IServiceCollection AddPowerShellServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IOutputStreamService, PowerShellOutputStreamService>();
        serviceCollection.TryAddScriptJobService();

        return serviceCollection;
    }
}