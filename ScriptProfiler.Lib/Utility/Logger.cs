namespace ScriptProfiler.Lib.Utility;

public static class Logger
{
    private const int MAX_LOG_FILES = 50;

    private static readonly string LogDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
    private static readonly string NewLine = Environment.NewLine;

    public static void AddMessages(LogEvent eventType, string[] messages)
    {
        foreach (string message in messages)
        {
            AddMessage(eventType, message);
        }
    }

    public static void AddMessage(LogEvent eventType, string message)
    {
        DirectoryInfo dirInfo = Directory.CreateDirectory(LogDirPath);

        if (dirInfo.Exists && FileHelper.GetFileCount(LogDirPath) >= MAX_LOG_FILES)
        {
            FileHelper.DeleteOldestFiles(dirInfo, 1);
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine(message);
#endif

        string timestamp = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
        string logMessage = $"[{timestamp}] => {eventType.ToString().ToUpper()}: {NewLine}{message}{NewLine}";

        try
        {
            File.WriteAllText(Path.Combine(LogDirPath, Guid.NewGuid() + ".log"), logMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to write to log file:");
            Console.WriteLine(e.Message);
        }
    }
}
