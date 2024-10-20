using Microsoft.Identity.Client;

namespace ScriptProfiler.Lib.Services.Authentication;

public interface IAuthenticationService
{
    public Task AuthenticateRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken);

    public Task<AuthenticationResult?> AcquireTokenSilentAsync(CancellationToken cancellationToken);

    public Task<AuthenticationResult?> AcquireTokenInteractiveAsync(CancellationToken cancellationToken);
}
