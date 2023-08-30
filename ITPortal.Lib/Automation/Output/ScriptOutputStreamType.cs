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
    public static readonly IReadOnlyDictionary<ScriptOutputStreamType, string> Colors = new Dictionary<ScriptOutputStreamType, string>()
    {
        { ScriptOutputStreamType.Standard,    StateColors.Black  },
        { ScriptOutputStreamType.Information, StateColors.Blue   },
        { ScriptOutputStreamType.Progress,    StateColors.Green  },
        { ScriptOutputStreamType.Warning,     StateColors.Yellow },
        { ScriptOutputStreamType.Error,       StateColors.Red    },
    };

    public static string GetColor(this ScriptOutputStreamType streamType)
    {
        return Colors[streamType];
    }

    public static string ToStringFast(this ScriptOutputStreamType streamType)
    {
        return streamType switch
        {
            ScriptOutputStreamType.Standard    => nameof(ScriptOutputStreamType.Standard),
            ScriptOutputStreamType.Information => nameof(ScriptOutputStreamType.Information),
            ScriptOutputStreamType.Progress    => nameof(ScriptOutputStreamType.Progress),
            ScriptOutputStreamType.Warning     => nameof(ScriptOutputStreamType.Warning),
            ScriptOutputStreamType.Error       => nameof(ScriptOutputStreamType.Error),
            _ => throw new ArgumentOutOfRangeException(nameof(streamType)),
        };
    }
}
