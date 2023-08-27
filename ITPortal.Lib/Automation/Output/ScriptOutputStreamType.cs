using ITPortal.Lib.Utilities;

namespace ITPortal.Lib.Automation.Output;

public enum ScriptOutputStreamType
{
    Standard,
    Information,
    Progress,
    Warning,
    Error
}

public static class ScriptOutputStreamTypeExtensions
{
    public static readonly Dictionary<ScriptOutputStreamType, string> Colors = new()
    {
        { ScriptOutputStreamType.Standard,    StateColors.Black  },
        { ScriptOutputStreamType.Information, StateColors.Blue   },
        { ScriptOutputStreamType.Progress,    StateColors.Green  },
        { ScriptOutputStreamType.Warning,     StateColors.Yellow },
        { ScriptOutputStreamType.Error,       StateColors.Red    },
    };

    public static string GetColor(this ScriptOutputStreamType state)
    {
        return Colors[state];
    }
}
