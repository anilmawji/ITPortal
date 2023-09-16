namespace ITPortal.Lib.Utilities;

public enum LogEvent
{
    Info,
    Success,
    Warning,
    Error
}

public static class Logger
{
    private static readonly string LogDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

    public static void WriteToFile(LogEvent eventType, string message)
    {
        if (!File.Exists(LogDirPath))
        {
            Directory.CreateDirectory(LogDirPath);
        }

        try
        {
            File.AppendAllText(Path.Combine(LogDirPath, Guid.NewGuid() + ".log"), NewLogMessage(eventType, message));
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to write to log file:");
            Console.WriteLine(e.Message);
        }
    }

    private static string NewLogMessage(LogEvent eventType, string message)
    {
        string timestamp = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
        string newLine = Environment.NewLine;

        return $"[{timestamp}] => {eventType.ToString().ToUpper()}: {newLine}{newLine}{message}{newLine}";
    }
}