using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace ITPortal.Lib.Services.Authentication;

public sealed class MsalAuthenticationService : IAuthenticationService
{
    private readonly AzureAdSettings _settings;
    private readonly IPublicClientApplication? _authenticationClient;

    public MsalAuthenticationService(AzureAdSettings configuration)
    {
        _settings = configuration;
        // Public client apps are not trusted to safely keep secrets, so only access web APIs on behalf of the user
        _authenticationClient = PublicClientApplicationBuilder.Create(_settings.ClientId)
            // Mark as accessible by Arcurve accounts only
            .WithAuthority(AzureCloudInstance.AzurePublic, _settings.TenantId)
            .WithRedirectUri($"msal{_settings.ClientId}://auth")
            .Build();
    }

    // Add access token to HTTP request headers
    public async Task AuthenticateRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AuthenticationResult? authResult = await AcquireTokenSilentAsync(cancellationToken);
        string? token = authResult?.AccessToken;

        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
    }

    public async Task<AuthenticationResult?> AcquireTokenSilentAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_authenticationClient);

        // Used by MSAL to search user token cache for a valid accessToken
        IEnumerable<IAccount> accounts = await _authenticationClient.GetAccountsAsync();

        AuthenticationResult? result;
        try
        {
            if (accounts.Any())
            {
                result = await _authenticationClient.AcquireTokenSilent(_settings.Scopes, accounts.FirstOrDefault())
                    .WithTenantId(_settings.TenantId)
                    .ExecuteAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                result = await AcquireTokenInteractiveAsync(cancellationToken);
            }
        }
        catch (MsalUiRequiredException)
        {
            // Acquiring silently failed; need to acquire the token interactively
            result = await AcquireTokenInteractiveAsync(cancellationToken);
        }

        return result;
    }

    public async Task<AuthenticationResult?> AcquireTokenInteractiveAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_authenticationClient);

        AuthenticationResult result;
        try
        {
            result = await _authenticationClient.AcquireTokenInteractive(_settings.Scopes)
                .WithTenantId(_settings.TenantId)
                .WithParentActivityOrWindow(DeviceWindowHandler.WindowHandle)
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
        catch (MsalClientException)
        {
            return null;
        }
    }
}
