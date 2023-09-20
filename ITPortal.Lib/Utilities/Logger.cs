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
    private static readonly string NewLine = Environment.NewLine;

    public static void AddMessage(LogEvent eventType, string message, bool printToDebug = true)
    {
        if (!File.Exists(LogDirPath))
        {
            Directory.CreateDirectory(LogDirPath);
        }

        if (printToDebug)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        string timestamp = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
        string logMessage = $"[{timestamp}] => {eventType.ToString().ToUpper()}: {NewLine}{message}{NewLine}";

        try
        {
            File.AppendAllText(Path.Combine(LogDirPath, Guid.NewGuid() + ".log"), logMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to write to log file:");
            Console.WriteLine(e.Message);
        }
    }
}