using Microsoft.Identity.Client;

namespace ITPortal.Lib.Services.Authentication;

public interface IAuthenticationService
{
    public Task AuthenticateRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken);

    public Task<AuthenticationResult?> AcquireTokenSilentAsync(CancellationToken cancellationToken);
    public Task<AuthenticationResult?> AcquireTokenInteractiveAsync(CancellationToken cancellationToken);
}
