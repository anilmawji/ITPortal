using ITPortal.Lib.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.Collections;

namespace ITPortal.Lib.Services.Automation.Output;

public abstract class ScriptOutputList : IEnumerable<ScriptOutputMessage>
{
    public event EventHandler<ScriptOutputChangedEventArgs>? OnOutputChanged;

    private List<ScriptOutputMessage> Output { get; set; } = new();
    private ScriptOutputMessage? _previousMessage;

    public abstract void SubscribeToOutputStream<T>(ICollection<T> stream, ScriptOutputStreamType streamType);

    public void Add(string? message, ScriptOutputStreamType streamType)
    {
        if (message.IsNullOrEmpty()) return;

        if (_previousMessage?.Data == message)
        {
            Output.Last().Data += ".";
        }
        else
        {
            ScriptOutputMessage psMessage = new()
            {
                Stream = streamType,
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

    public bool DisposeOnOutputChangedEventSubscriptions()
    {
        return OnOutputChanged.DisposeSubscriptions();
    }

    public IEnumerator<ScriptOutputMessage> GetEnumerator()
    {
        return Output.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Output.GetEnumerator();
    }
}
