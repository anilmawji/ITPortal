using ITPortal.Lib.Services.Event;
using Microsoft.IdentityModel.Tokens;
using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation.Output;

public class PowerShellService : IOutputStreamService
{
    public List<OutputMessage> Output { get; set; } = new();

    public event EventHandler<ScriptOutputChangedEventArgs>? OnOutputChanged;

    private OutputMessage? previousMessage;

    public void SubscribeToStream<T>(ICollection<T> stream, OutputStreamType streamType)
    {
        var psStream = (PSDataCollection<T>)stream;

        psStream.DataAdded += (sender, e) =>
        {
            string? message = psStream[e.Index]?.ToString();
            AddOutput(message, streamType);
        };
    }

    public void AddOutput(string? message, OutputStreamType streamType)
    {
        if (message.IsNullOrEmpty() || OnOutputChanged == null) return;

        if (previousMessage?.Data == message)
        {
            Output.Last().Data += ".";
        }
        else
        {
            OutputMessage psMessage = new()
            {
                Stream = streamType,
                Data = message
            };

            Output.Add(psMessage);
            previousMessage = psMessage;
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
}
