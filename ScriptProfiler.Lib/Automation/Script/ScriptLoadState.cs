namespace ScriptProfiler.Lib.Automation.Script;

public enum ScriptLoadState
{
    Unloaded,
    Failed,
    Success
}

public static class ScriptLoadStateExtensions
{
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
