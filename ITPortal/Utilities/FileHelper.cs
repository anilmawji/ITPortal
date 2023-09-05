using System.Diagnostics;

namespace ITPortal.Utilities;

public sealed class FileHelper
{
    // fileName must include the file's name and the file extension
    public static void WriteTextToAppDataFile(string jsonText, string filePath)
    {
        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, filePath);

        File.WriteAllText(fullPath, jsonText);
    }

    // fileName must include the file's name and the file extension
    public static string ReadTextFromAppDataFile(string filePath)
    {
        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, filePath);

        return File.ReadAllText(fullPath);
    }

    public static bool TryDeleteAppDataFile(string filePath)
    {
        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, filePath);

        if (!File.Exists(fullPath)) return false;

        File.Delete(fullPath);

        return true;
    }

    public static DirectoryInfo TryCreateAppDataDirectory(string directoryPath)
    {
        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, directoryPath);

        return Directory.CreateDirectory(fullPath);
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
        using Process process = Process.Start("explorer", "\"" + filePath + "\"");
    }
}
