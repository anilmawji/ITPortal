using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ITPortal.Lib.Services.Authentication;

public class ExternalAuthStateProvider : AuthenticationStateProvider
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

    public async Task LogInAsync()
    {
        SetAuthenticationStateAsync(await LoginWithExternalProviderAsync());
    }

    private async Task<AuthenticatedUser> LoginWithExternalProviderAsync()
    {
        var authResult = await _authenticationService.AcquireTokenInteractiveAsync();

        // Authentication failed, return a logged out user state
        if (authResult == null) return new AuthenticatedUser();

        AuthenticatedUser authenticatedUser = new(authResult.ClaimsPrincipal);
        // For some reason AAD uses "name" as the claim type instead of ClaimTypes.Name
        // The user context only recognizes ClaimTypes.Name
        authenticatedUser.UpdateClaimType("name", ClaimTypes.Name);

        return authenticatedUser;
    }

    public void Logout()
    {
        SetAuthenticationStateAsync(new AuthenticatedUser());
    }
}
