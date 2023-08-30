using ITPortal.Lib.Utilities;

namespace ITPortal.Lib.Automation.Job;

public enum ScriptJobState
{
    Idle,
    Scheduled,
    Running
}

public static class ScriptJobStateMethods
{
    public static readonly IReadOnlyDictionary<ScriptJobState, string> Colors = new Dictionary<ScriptJobState, string>()
    {
        { ScriptJobState.Idle,      StateColors.Grey },
        { ScriptJobState.Scheduled, StateColors.Grey },
        { ScriptJobState.Running,   StateColors.Blue },
    };

    public static string GetColor(this ScriptJobState state)
    {
        return Colors[state];
    }

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
