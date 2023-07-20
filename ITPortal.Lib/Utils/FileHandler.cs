using System.Diagnostics;
using System.Text;

namespace ITPortal.Lib.Utils;

public class FileHandler
{
    public static string GetFileContent(string filePath)
    {
        string text;

        using (var streamReader = new StreamReader(filePath, Encoding.UTF8))
        {
            text = streamReader.ReadToEnd();
        }
        return text;
    }

    public static void OpenFileWithDefaultProgram(string filePath)
    {
        using Process process = Process.Start("explorer", "\"" + filePath + "\"");
    }
}
