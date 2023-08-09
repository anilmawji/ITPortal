using ITPortal.Lib.Services.Authentication.External;
using ITPortal.Lib.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using ITPortal.Lib.Services.Automation.JobTest;
using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation;

public static class DependencyInjectionExtensions
{
    private static bool ScriptJobServiceAdded;

    private static IServiceCollection TryAddScriptJobService(this IServiceCollection serviceCollection)
    {
        if (!ScriptJobServiceAdded)
        {
            serviceCollection.AddSingleton<ScriptJobService>();
            ScriptJobServiceAdded = true;
        }
        return serviceCollection;
    }

    public static IServiceCollection AddPowerShellServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IScriptOutputStreamService, PowerShellOutputStreamService>();
        serviceCollection.TryAddScriptJobService();

        return serviceCollection;
    }
}
