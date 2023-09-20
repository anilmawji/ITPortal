using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Client;

namespace ITPortal.Lib.Services.Authentication;

public sealed class ExternalAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthenticationService _authenticationService;
    private AuthenticatedUser _currentUser;

    public ExternalAuthStateProvider(IAuthenticationService authenticationService)
    {
        _currentUser = new AuthenticatedUser();
        _authenticationService = authenticationService;
    }

    public void SetAuthenticationStateAsync(AuthenticatedUser user)
    {
        _currentUser = user;
        // Use BlazorWebView to update auth state
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(_currentUser.Principal));

    public async Task LogInAsync(CancellationToken cancellationToken = default)
    {
        SetAuthenticationStateAsync(await LoginWithExternalProviderAsync(cancellationToken));
    }

    private async Task<AuthenticatedUser> LoginWithExternalProviderAsync(CancellationToken cancellationToken = default)
    {
        AuthenticationResult? authResult = await _authenticationService.AcquireTokenInteractiveAsync(cancellationToken);

        if (authResult == null)
        {
            // Authentication failed, return a logged out user state
            return new AuthenticatedUser();
        }
        AuthenticatedUser user = new(authResult.ClaimsPrincipal);

        // For some reason AAD uses "name" as the claim type instead of ClaimTypes.Name
        // The user context only recognizes ClaimTypes.Name
        Dictionary<string, string> claimTypeReplacements = new()
        {
            { "name", ClaimTypes.Name },
            { "preferred_username", ClaimTypes.Email }
        };
        user.ReplaceClaimTypes(claimTypeReplacements);

        return user;
    }

    public void Logout()
    {
        SetAuthenticationStateAsync(new AuthenticatedUser());
    }
}
