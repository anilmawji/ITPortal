namespace ScriptProfiler.Lib.Automation.Job;

public enum ScriptJobState
{
    Idle,
    Scheduled,
    Running
}

public static class ScriptJobStateExtensions
{
    public static string ToStringFast(this ScriptJobState state)
    {
        return state switch
        {
            ScriptJobState.Idle      => nameof(ScriptJobState.Idle),
            ScriptJobState.Scheduled => nameof(ScriptJobState.Scheduled),
            ScriptJobState.Running   => nameof(ScriptJobState.Running),
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        };
    }
}
