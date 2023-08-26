namespace ITPortal.Lib.Services.Automation.Job;

public enum ScriptExecutionResult
{
    Running,
    Success,
    Stopped,
    Error
}

public static class ScriptExecutionStateExtensions
{
    public static readonly Dictionary<ScriptExecutionResult, string> Colors = new()
    {
        { ScriptExecutionResult.Running, "#0094FF" }, // Blue
        { ScriptExecutionResult.Success, "#00C708" }, // Green
        { ScriptExecutionResult.Error,   "#BF0000" }, // Red
        { ScriptExecutionResult.Stopped, "#FCBA03" }, // Yellow
    };

    public static string GetColor(this ScriptExecutionResult state)
    {
        return Colors[state];
    }
}
