using ITPortal.Lib.Utility;

namespace ITPortal.Lib.Automation.Script;

public enum ScriptLoadState
{
    Unloaded,
    Failed,
    Success
}

public static class ScriptLoadStateExtensions
{
    public static readonly IReadOnlyDictionary<ScriptLoadState, string> Colors = new Dictionary<ScriptLoadState, string>()
    {
        { ScriptLoadState.Unloaded, StateColors.Grey  },
        { ScriptLoadState.Success,  StateColors.Green },
        { ScriptLoadState.Failed,   StateColors.Red   },
    };

    public static string GetColor(this ScriptLoadState state)
    {
        return Colors[state];
    }

    public static string ToStringFast(this ScriptLoadState state)
    {
        return state switch
        {
            ScriptLoadState.Unloaded => nameof(ScriptLoadState.Unloaded),
            ScriptLoadState.Success  => nameof(ScriptLoadState.Success),
            ScriptLoadState.Failed   => nameof(ScriptLoadState.Failed),
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        };
    }
}
