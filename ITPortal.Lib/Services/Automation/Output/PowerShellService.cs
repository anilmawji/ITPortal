using ITPortal.Lib.Services.Automation.Job;
using Microsoft.IdentityModel.Tokens;
using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation.Output;

public class PowerShellService : IOutputStreamService
{
    public List<OutputMessage> Output { get; set; } = new();

    public event EventHandler<ScriptOutputChangedEventArgs>? OutputChanged;

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
        if (message.IsNullOrEmpty() || OutputChanged == null) return;

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
        OutputChanged?.Invoke(this, args);
    }

    public void DisposeEventSubscriptions()
    {
        if (OutputChanged != null)
        {
            foreach (Delegate d in OutputChanged.GetInvocationList())
            {
                OutputChanged -= (EventHandler<ScriptOutputChangedEventArgs>)d;
            }
        }
    }
}
