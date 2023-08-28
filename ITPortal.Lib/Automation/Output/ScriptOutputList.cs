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
        if (message.IsNullOrEmpty()) return;

        UsedStreamTypes[streamType]++;

        if (_previousMessage?.Data == message)
        {
            Output.Last().Data += ".";
        }
        else
        {
            ScriptOutputMessage psMessage = new()
            {
                StreamType = streamType,
                Data = message
            };
            Output.Add(psMessage);
            _previousMessage = psMessage;
        }

        ScriptOutputChangedEventArgs args = new()
        {
            Output = Output,
            StreamType = streamType
        };
        OnOutputChanged?.Invoke(this, args);
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
