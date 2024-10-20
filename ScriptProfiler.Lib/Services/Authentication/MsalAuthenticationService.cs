using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace ScriptProfiler.Lib.Services.Authentication;

public sealed class MsalAuthenticationService : IAuthenticationService
{
    private readonly AzureAdSettings _azureAdSettings;
    private readonly IPublicClientApplication? _authenticationClient;

    public MsalAuthenticationService(IOptions<AzureAdSettings> azureAdSettingsAccessor)
    {
        _azureAdSettings = azureAdSettingsAccessor.Value;
        // Public client apps are not trusted to safely keep secrets, so only access web APIs on behalf of the user
        _authenticationClient = PublicClientApplicationBuilder.Create(_azureAdSettings.ClientId)
            // Mark as accessible by Arcurve accounts only
            .WithAuthority(AzureCloudInstance.AzurePublic, _azureAdSettings.TenantId)
            .WithRedirectUri($"msal{_azureAdSettings.ClientId}://auth")
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
                result = await _authenticationClient.AcquireTokenSilent(_azureAdSettings.Scopes, accounts.FirstOrDefault())
                    .WithTenantId(_azureAdSettings.TenantId)
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
            result = await _authenticationClient.AcquireTokenInteractive(_azureAdSettings.Scopes)
                .WithTenantId(_azureAdSettings.TenantId)
                .WithParentActivityOrWindow(DeviceWindowHelper.WindowHandle)
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
