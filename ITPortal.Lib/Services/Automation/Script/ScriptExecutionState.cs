namespace ITPortal.Lib.Services.Automation.Job;

public enum ScriptExecutionState
{
    Running,
    Success,
    Stopped,
    Error
}

public static class ScriptExecutionStateExtensions
{
    public static string GetColor(this ScriptExecutionState state)
    {
        return state == ScriptExecutionState.Success ? "#00C708"    // Green
            : state == ScriptExecutionState.Error ? "#BF0000"       // Red
            : state == ScriptExecutionState.Stopped ? "#FCBA03"     // Yellow
            : "#747474";                                            // Grey
    }
}
