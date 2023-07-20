using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ITPortal.Lib.Services;

public class GraphClientService : IGraphClientService
{
    public GraphServiceClient Client { get; set; }

    public GraphClientService(IAccessTokenProvider tokenProvider)
    {
        var authenticationProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
        Client = new GraphServiceClient(authenticationProvider);
    }

    public async Task<User?> GetUserAsync()
    {
        try
        {
            return await Client.Me.GetAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
