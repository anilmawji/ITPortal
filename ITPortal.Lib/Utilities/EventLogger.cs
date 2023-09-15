namespace ITPortal.Lib.Utilities;

enum LogEvent
{
    Info,
    Success,
    Warning,
    Error
}

internal static class EventLogger
{
    private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

    internal static void Write(LogEvent eventType, string message)
    {
        File.WriteAllText(
            Path.Combine(LogPath, Guid.NewGuid() + ".log"),
            string.Format("[{0}] => {1}:\n\n{2}\n", DateTime.Now.ToLocalTime().ToString("HH:mm:ss"), eventType.ToString().ToUpper(), message)
        );
    }
}