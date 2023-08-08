using Microsoft.IdentityModel.Tokens;
using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation.Output;

public class PowerShellOutputStreamService : IScriptOutputStreamService
{
    public List<ScriptOutputMessage> Output { get; set; } = new();
    public event EventHandler<List<ScriptOutputMessage>>? OutputChanged;

    private ScriptOutputMessage prevMessage = new();

    public void SubscribeToOutputStream<T>(ICollection<T> stream, ScriptStreamType streamType)
    {
        var psStream = (PSDataCollection<T>)stream;
        psStream.DataAdded += (sender, e) =>
        {
            string? message = psStream[e.Index]?.ToString();
            AddOutput(streamType, message);
        };
    }

    public void AddOutput(string? message)
    {
        AddOutput(ScriptStreamType.Output, message);
    }

    public void AddOutput(ScriptStreamType streamType, string? message)
    {
        if (message.IsNullOrEmpty() || OutputChanged == null) return;

        if (prevMessage.Data == message)
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
            prevMessage = psMessage;
        }
        // Execute callback function to update the UI
        OutputChanged?.Invoke(this, Output);
    }
}
