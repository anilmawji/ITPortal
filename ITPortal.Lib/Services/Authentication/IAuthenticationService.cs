using Microsoft.Identity.Client;

namespace ITPortal.Lib.Services.Authentication;

public interface IAuthenticationService
{
    public Task<AuthenticationResult?> AcquireTokenSilentAsync();

    public Task<AuthenticationResult?> AcquireTokenInteractiveAsync();
}
