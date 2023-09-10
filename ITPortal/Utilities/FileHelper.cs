using System.Diagnostics;

namespace ITPortal.Utilities;

public sealed class FileHelper
{
    public static readonly string ScriptJobsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "jobs");
    public static readonly string ScriptJobResultsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "job_results");

    public static bool TryCreateFile(string fileName, string folderPath, string fileContents)
    {
        try
        {
            File.WriteAllText(Path.Combine(folderPath, fileName), fileContents);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool TryDeleteFile(string fileName, string folderPath)
    {
        string filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        return false;
    }

    public static async Task<FileResult> PickFile(PickOptions options)
    {
        try
        {
            // Prompt user to pick file
            FileResult result = await FilePicker.PickAsync(options);

            if (result == null)
            {
                return null;
            }
            foreach (string fileType in options.FileTypes.Value)
            {
                if (result.FileName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                {
                    return result;
                }
            }
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    public static void OpenFileWithDefaultProgram(string filePath)
    {
        using Process process = Process.Start("explorer", filePath);
    }
}
