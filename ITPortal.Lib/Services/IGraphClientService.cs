using Microsoft.Graph.Models;

namespace ITPortal.Lib.Services;

public interface IGraphClientService
{
    public Task<User?> GetUserAsync();
}
