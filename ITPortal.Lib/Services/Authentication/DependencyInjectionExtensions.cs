using ITPortal.Lib.Services.Authentication.External;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ITPortal.Lib.Services.Authentication;

public static class DependencyInjectionExtensions
{
    private static bool AuthorizationCoreAdded;

    private static IServiceCollection TryAddAuthorizationCore(this IServiceCollection serviceCollection)
    {
        if (!AuthorizationCoreAdded)
        {
            serviceCollection.AddAuthorizationCore();
            AuthorizationCoreAdded = true;
        }
        return serviceCollection;
    }

    public static IServiceCollection AddMsalAuthentication(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddAuthorizationCore();
        serviceCollection.TryAddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();
        serviceCollection.AddSingleton<IAuthenticationService, MsalAuthenticationService>();
        serviceCollection.TryAddSingleton<IAccessTokenProvider, TokenProvider>();

        return serviceCollection;
    }
}
