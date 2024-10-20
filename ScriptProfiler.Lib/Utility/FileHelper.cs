using System.Diagnostics;

namespace ScriptProfiler.Lib.Utility;

public class FileHelper
{
    public static void OpenFileWithDefaultProgram(string filePath)
    {
        using Process process = Process.Start("explorer", filePath);
    }

    public static void DeleteOldestFiles(DirectoryInfo dirInfo, int numFiles)
    {
        List<FileInfo> files = dirInfo.EnumerateFiles()
            .OrderByDescending(f => f.CreationTime)
            .Take(numFiles)
            .ToList();

        files.ForEach(f => f.Delete());
    }

    public static int GetFileCount(string dirPath)
    {
        return Directory.EnumerateFiles(dirPath).Count();
    }
}
