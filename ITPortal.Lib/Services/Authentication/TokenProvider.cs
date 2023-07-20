using Microsoft.Kiota.Abstractions.Authentication;

namespace ITPortal.Lib.Services.Authentication;

public class TokenProvider : IAccessTokenProvider
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
        var authResult = await _authenticationService.AcquireTokenSilentAsync();

        string? token = authResult?.AccessToken;

        // Token is never null, null check is to avoid nullability warning
        if (token == null)
        {
            return string.Empty;
        }
        return token;
    }
}
