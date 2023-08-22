namespace ITPortal.Lib.Services.Automation.Job;

public enum ScriptJobState
{
    Idle,
    Scheduled,
    Running
}

public static class ScriptJobStateMethods
{
    public static readonly Dictionary<ScriptJobState, string> Colors = new()
    {
        { ScriptJobState.Idle,      "#747474" }, // Grey
        { ScriptJobState.Scheduled, "#747474" }, // Grey
        { ScriptJobState.Running,   "#0094FF" }, // Blue
    };

    public static string GetColor(this ScriptJobState state)
    {
        return Colors[state];
    }
}
