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
    public static readonly IReadOnlyDictionary<ScriptExecutionState, string> Colors = new Dictionary<ScriptExecutionState, string>()
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

    public static string ToStringFast(this ScriptExecutionState state)
    {
        return state switch
        {
            ScriptExecutionState.Running => nameof(ScriptExecutionState.Running),
            ScriptExecutionState.Success => nameof(ScriptExecutionState.Success),
            ScriptExecutionState.Stopped => nameof(ScriptExecutionState.Stopped),
            ScriptExecutionState.Error => nameof(ScriptExecutionState.Error),
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        };
    }
}
