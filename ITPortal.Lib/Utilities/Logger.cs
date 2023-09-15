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
    private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

    public static void Write(LogEvent eventType, string message)
    {
        File.WriteAllText(
            Path.Combine(LogPath, Guid.NewGuid() + ".log"),
            string.Format("[{0}] => {1}:\n\n{2}\n", DateTime.Now.ToLocalTime().ToString("HH:mm:ss"), eventType.ToString().ToUpper(), message)
        );
    }
}