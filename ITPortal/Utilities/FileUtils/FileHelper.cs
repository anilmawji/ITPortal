using System.Diagnostics;

namespace ITPortal.Utilities.FileUtils;

public sealed class FileHelper
{
    public static bool CreateFile(string fileName, string fileExtension, string folderPath, string fileContents)
    {
        try
        {
            File.WriteAllText(Path.Combine(folderPath, fileName + "." + fileExtension), fileContents);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool DeleteFile(string fileName, string fileExtension, string folderPath)
    {
        string filePath = Path.Combine(folderPath, fileName + "." + fileExtension);

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
