using Microsoft.Graph.Models;

namespace ScriptProfiler.Lib.Services;

public interface IGraphClientService
{
    public Task<User?> GetUserAsync();
}
