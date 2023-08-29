using ITPortal.Lib.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace ITPortal.Lib.Automation.Output;

public abstract class ScriptOutputList : IDisposable
{
    public event EventHandler<ScriptOutputChangedEventArgs>? OnOutputChanged;
    public readonly Dictionary<ScriptOutputStreamType, int> UsedStreamTypes = EnumHelper.ToDictionary<ScriptOutputStreamType, int>(0);

    private List<ScriptOutputMessage> Output { get; set; } = new();
    private ScriptOutputMessage? _previousMessage;

    public abstract void SubscribeToOutputStream<T>(ICollection<T> stream, ScriptOutputStreamType streamType);

    public void Add(string? message, ScriptOutputStreamType streamType)
    {
        // message.IsNullOrEmpty() is not recognized by Roslyn as guarding against null....
        if (message == null || message == string.Empty) return;

        UsedStreamTypes[streamType]++;

        if (_previousMessage?.Data == message)
        {
            Output.Last().Data += ".";
        }
        else
        {
            SendOutputMessage(message, streamType);
        }

        ScriptOutputChangedEventArgs args = new()
        {
            Output = Output,
            StreamType = streamType
        };
        OnOutputChanged?.Invoke(this, args);
    }

    public void SendOutputMessage(string message, ScriptOutputStreamType streamType)
    {
        ScriptOutputMessage outputMessage = new()
        {
            StreamType = streamType,
            Data = message
        };
        Output.Add(outputMessage);
        _previousMessage = outputMessage;
    }

    public void SendSystemMessage(string message, ScriptOutputStreamType streamType)
    {
        SendOutputMessage(FormatAsSystemMessage(message), streamType);
    }

    public static string FormatAsSystemMessage(string message)
    {
        return "[SYSTEM]: " + message;
    }

    public IReadOnlyList<ScriptOutputMessage> Get()
    {
        return Output.AsReadOnly();
    }

    public IReadOnlyList<ScriptOutputMessage> GetFilteredByStreamType(ScriptOutputStreamType streamType)
    {
        return Output.Where(x => x.StreamType == streamType).ToList().AsReadOnly();
    }

    public void Clear()
    {
        Output.Clear();
    }

    public bool DisposeOnOutputChangedEventSubscriptions()
    {
        return OnOutputChanged.DisposeSubscriptions();
    }

    public void Dispose()
    {
        OnOutputChanged.DisposeSubscriptions();
    }
}
