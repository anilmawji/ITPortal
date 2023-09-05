using System.Diagnostics;

namespace ITPortal.Utilities;

public sealed class FileHelper
{
    public static readonly string ScriptJobsFolderPath = Path.Combine(FileSystem.Current.AppDataDirectory, "jobs");

    public static bool TryDeleteFile(string filePath)
    {
        if (!File.Exists(filePath)) return false;

        File.Delete(filePath);

        return true;
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
