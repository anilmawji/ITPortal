using ITPortal.Lib.Models.GitHub;
using ITPortal.Lib.Services.Core;
using ITPortal.Lib.Utilities.Extensions;
using System.Net.Http.Headers;

namespace ITPortal.Lib.Services;

public sealed class GitHubService : IGitHubService
{
    private readonly IHttpClientService? _httpClient;
    private string? errorMessage;

    public GitHubService(IHttpClientService httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Client.BaseAddress = new Uri("https://api.github.com");

        // TODO: Retrieve from Key Vault
        string token = "";

        _httpClient.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
    }

    public async Task<FileModel?> GetFile(string user, string repo, string filePath)
    {
        if (_httpClient == null) return null;

        FileModel? file;

        try
        {
            file = await _httpClient.GetModelAsync<FileModel>($"/repos/{user}/{repo}/contents/{filePath}")
                .ConfigureAwait(false);

            errorMessage = null;
        }
        catch (HttpRequestException e)
        {
            file = null;
            errorMessage = $"There was an error getting the file data: {e.Message}";
        }

        return file;
    }

    public string? GetFileContent(FileModel? file)
    {
        return file?.content.DecodeBase64();
    }

    public async Task<List<string>?> GetFilePaths(string user, string repo, string branch, string options, string extension)
    {
        if (_httpClient == null) return null;

        BranchModel? repoBranch;
        FileTreeModel? fileTrees;
        List<string> filePaths = new();
        
        try
        {
            repoBranch = await _httpClient.GetModelAsync<BranchModel>($"/repos/{user}/{repo}/branches/{branch}")
                .ConfigureAwait(false);

            if (repoBranch == null)
            {
                return null;
            }

            string treeUrl = repoBranch.commit.commit.tree.url;
            fileTrees = await _httpClient.GetModelAsync<FileTreeModel>($"{treeUrl}?{options}")
                .ConfigureAwait(false);

            if (fileTrees == null)
            {
                return null;
            }

            Tree? fileTree = null;

            if (extension != null)
            {
                for (int i = 0; i < fileTrees.tree.Length; i++)
                {
                    fileTree = fileTrees.tree[i];
                    if (fileTree.path.EndsWith(extension))
                    {
                        filePaths.Add(fileTree.path);
                    }
                }
            }
            else
            {
                for (int i = 0; i < fileTrees.tree.Length; i++)
                {
                    fileTree = fileTrees.tree[i];
                    filePaths.Add(fileTree.path);
                }
            }

            errorMessage = null;
        }
        catch (HttpRequestException e)
        {
            errorMessage = $"There was an error getting the file data: {e.Message}";
        }

        return filePaths;
    }

    public string? GetErrorMessage()
    {
        return errorMessage;
    }
}
