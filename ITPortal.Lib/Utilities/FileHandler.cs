using System.Diagnostics;

namespace ITPortal.Lib.Utilities;

public sealed class FileHandler
{
    public static void OpenFileWithDefaultProgram(string filePath)
    {
        using Process process = Process.Start("explorer", "\"" + filePath + "\"");
    }
}
