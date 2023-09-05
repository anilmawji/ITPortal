using System.Diagnostics;

namespace ITPortal.Utilities;

public sealed class FileHelper
{
    // fileName must include the file's name and the file extension
    public static void WriteTextToAppDataFile(string jsonText, string fileName)
    {
        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

        File.WriteAllText(fullPath, jsonText);
    }

    // fileName must include the file's name and the file extension
    public static string ReadTextFromAppDataFile(string fileName)
    {
        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

        return File.ReadAllText(fullPath);
    }

    public static bool TryDeleteAppDataFile(string fileName)
    {
        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

        if (!File.Exists(fullPath)) return false;

        File.Delete(fullPath);

        return true;
    }

    public static DirectoryInfo CreateAppDataDirectory(string directoryName)
    {
        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, directoryName);

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
