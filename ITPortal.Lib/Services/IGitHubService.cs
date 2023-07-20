using ITPortal.Lib.Models.GitHub;

namespace ITPortal.Lib.Services;

public interface IGitHubService
{
    Task<FileModel?> GetFile(string user, string repo, string filePath);

    string? GetFileContent(FileModel? file);
}
