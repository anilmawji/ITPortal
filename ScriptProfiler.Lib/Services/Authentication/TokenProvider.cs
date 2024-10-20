using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ScriptProfiler.Lib.Services.Authentication;

public sealed class TokenProvider : IAccessTokenProvider
{
    // TODO: Required as part of IAccessTokenProvider
    public AllowedHostsValidator AllowedHostsValidator => throw new NotImplementedException();

    private readonly IAuthenticationService _authenticationService;

    public TokenProvider(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    // Required as part of IAccessTokenProvider
    public async Task<string> GetAuthorizationTokenAsync(Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        AuthenticationResult? authResult = await _authenticationService.AcquireTokenSilentAsync(cancellationToken);

        if (authResult == null)
        {
            return string.Empty;
        }
        return authResult.AccessToken;
    }
}
