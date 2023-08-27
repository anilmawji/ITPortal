using ITPortal.Lib.Utilities;

namespace ITPortal.Lib.Automation.Script;

public enum ScriptExecutionState
{
    Running,
    Success,
    Stopped,
    Error
}

public static class ScriptExecutionStateExtensions
{
    public static readonly Dictionary<ScriptExecutionState, string> Colors = new()
    {
        { ScriptExecutionState.Running, StateColors.Blue   },
        { ScriptExecutionState.Success, StateColors.Green  },
        { ScriptExecutionState.Stopped, StateColors.Yellow },
        { ScriptExecutionState.Error,   StateColors.Red    },
    };

    public static string GetColor(this ScriptExecutionState state)
    {
        return Colors[state];
    }
}
