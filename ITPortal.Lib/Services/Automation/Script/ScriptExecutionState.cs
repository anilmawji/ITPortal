namespace ITPortal.Lib.Services.Automation.Job;

public enum ScriptExecutionState
{
    Idle,
    Running,
    Success,
    Stopped,
    Error
}

public static class ScriptExecutionStateExtensions
{
    public static readonly Dictionary<ScriptExecutionState, string> Colors = new()
    {
        { ScriptExecutionState.Idle,    "#747474" }, // Grey
        { ScriptExecutionState.Running, "#0094FF" }, // Blue
        { ScriptExecutionState.Success, "#00C708" }, // Green
        { ScriptExecutionState.Error,   "#BF0000" }, // Red
        { ScriptExecutionState.Stopped, "#FCBA03" }, // Yellow
    };

    public static string GetColor(this ScriptExecutionState state)
    {
        return Colors[state];
    }
}
