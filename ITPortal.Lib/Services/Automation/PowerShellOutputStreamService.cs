using Microsoft.IdentityModel.Tokens;
using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation;

public class PowerShellOutputStreamService : IOutputStreamService<PSMessage, PSStream>
{
    public List<PSMessage> Output { get; set; }
    public event EventHandler<List<PSMessage>>? OutputChanged;

    private PSMessage prevMessage;

    public PowerShellOutputStreamService()
    {
        Output = new();
        prevMessage = new();
    }

    public void SubscribeToPowerShellStream<T>(PSDataCollection<T> stream, PSStream streamType)
    {
        stream.DataAdded += (object? sender, DataAddedEventArgs e) =>
        {
            var message = stream[e.Index]?.ToString();
            AddOutput(streamType, message);
        };
    }

    public void AddOutput(string? message)
    {
        AddOutput(PSStream.Output, message);
    }

    public void AddOutput(PSStream streamType, string? message)
    {
        if (message.IsNullOrEmpty()) return;
        if (OutputChanged == null) return;

        if (prevMessage.Data == message)
        {
            Output.Last().Data += ".";
        }
        else
        {
            PSMessage psMessage = new()
            {
                Stream = streamType,
                Data = message
            };

            Output.Add(psMessage);
            prevMessage = psMessage;
        }

        // Execute the callback function to update the UI
        OutputChanged?.Invoke(this, Output);
    }
}
