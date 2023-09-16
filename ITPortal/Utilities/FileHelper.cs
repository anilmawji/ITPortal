using ITPortal.Lib.Utilities;
using System.Diagnostics;
using System.Text;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ITPortal.Utilities;

public sealed class FileHelper
{
    public static bool CreateFile(string fileName, string folderPath, string fileContents)
    {
        try
        {
            File.WriteAllText(Path.Combine(folderPath, fileName), fileContents);
            return true;
        }
        catch (Exception e)
        {
            Logger.WriteToFile(LogEvent.Error, e.Message);
            return false;
        }
    }

    public static bool DeleteFile(string fileName, string folderPath)
    {
        string filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (Exception e)
        {
            Logger.WriteToFile(LogEvent.Error, e.Message);
            return false;
        }
    }

    public static async Task<FileResult> PickFileAsync(PickOptions options)
    {
        try
        {
            FileResult result = await FilePicker.PickAsync(options);

            if (result == null)
            {
                return null;
            }
            if (FileHasValidExtension(result.FileName, options))
            {
                return result;
            }
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    public static bool FileHasValidExtension(string fileName, PickOptions options)
    {
        foreach (string fileType in options.FileTypes.Value)
        {
            if (fileName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public static async ValueTask PickSaveFileAsync(string filename, Stream stream)
    {
        string extension = Path.GetExtension(filename);
        FileSavePicker fileSavePicker = new()
        {
            SuggestedFileName = filename
        };
        fileSavePicker.FileTypeChoices.Add(extension, new List<string> { extension });

        if (MauiWinUIApplication.Current.Application.Windows[0].Handler.PlatformView is MauiWinUIWindow window)
        {
            WinRT.Interop.InitializeWithWindow.Initialize(fileSavePicker, window.WindowHandle);
        }

        StorageFile result = await fileSavePicker.PickSaveFileAsync();

        if (result != null)
        {
            using Stream fileStream = await result.OpenStreamForWriteAsync();
            fileStream.SetLength(0); // override
            await stream.CopyToAsync(fileStream);
        }
    }

    public static ValueTask PickSaveFileAsync(string filename, string fileContent)
    {
        var stream = new MemoryStream(Encoding.Default.GetBytes(fileContent));

        return PickSaveFileAsync(filename, stream);
    }

    public static void OpenFileWithDefaultProgram(string filePath)
    {
        using Process process = Process.Start("explorer", filePath);
    }
}
