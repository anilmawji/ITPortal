using ScriptProfiler.Lib.Models.GitHub;

namespace ScriptProfiler.Lib.Services;

public interface IGitHubService
{
    Task<FileModel?> GetFile(string user, string repo, string filePath);

    string? GetFileContent(FileModel? file);
}
