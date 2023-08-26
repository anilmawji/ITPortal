using ITPortal.Lib.Utilities;

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
        { ScriptJobState.Idle,      StateColors.Grey },
        { ScriptJobState.Scheduled, StateColors.Grey },
        { ScriptJobState.Running,   StateColors.Blue },
    };

    public static string GetColor(this ScriptJobState state)
    {
        return Colors[state];
    }
}
