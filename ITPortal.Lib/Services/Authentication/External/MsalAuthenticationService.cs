using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace ITPortal.Lib.Services.Authentication.External;

public class MsalAuthenticationService : IAuthenticationService
{
    private readonly IPublicClientApplication? _authenticationClient;
    private readonly string[]? _scopes;
    private readonly string? _tenantId;
    private readonly string? _clientId;

    public MsalAuthenticationService(IConfiguration configuration)
    {
        _clientId = configuration["AzureAd:ClientId"];
        _tenantId = configuration["AzureAd:TenantId"];
        _scopes = configuration.GetValue<string[]>("AzureAd:Scopes");

        // Public client apps are not trusted to safely keep secrets, so only access web APIs on behalf of the user
        _authenticationClient = PublicClientApplicationBuilder.Create(_clientId)
            // Mark as accessible by Arcurve accounts only
            .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
            .WithRedirectUri($"msal{_clientId}://auth")
            .Build();
    }

    // Add access token to HTTP request headers
    public async Task AuthenticateRequestAsync(HttpRequestMessage request)
    {
        var authResult = await AcquireTokenSilentAsync();
        string? token = authResult?.AccessToken;

        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
    }

    public async Task<AuthenticationResult?> AcquireTokenSilentAsync()
    {
        ArgumentNullException.ThrowIfNull(_authenticationClient);

        // Used by MSAL to search user token cache for a valid accessToken
        var accounts = await _authenticationClient.GetAccountsAsync();

        AuthenticationResult? result;
        try
        {
            if (accounts.Any())
            {
                result = await _authenticationClient.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                    .WithTenantId(_tenantId)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                result = await AcquireTokenInteractiveAsync();
            }
        }
        catch (MsalUiRequiredException)
        {
            // Acquiring silently failed; need to acquire the token interactively
            result = await AcquireTokenInteractiveAsync();
        }

        return result;
    }

    public async Task<AuthenticationResult?> AcquireTokenInteractiveAsync()
    {
        ArgumentNullException.ThrowIfNull(_authenticationClient);

        AuthenticationResult result;
        try
        {
            result = await _authenticationClient.AcquireTokenInteractive(_scopes)
                .WithTenantId(_tenantId)
                .WithParentActivityOrWindow(DeviceWindowHandler.WindowHandle)
                .ExecuteAsync()
                .ConfigureAwait(false);

            return result;
        }
        catch (MsalClientException)
        {
            return null;
        }
    }
}
