using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation.Output;

public class PowerShellOutputCollection : ScriptOutputCollection
{
    public override void SubscribeToOutputStream<T>(ICollection<T> stream, ScriptOutputStreamType streamType)
    {
        var psStream = (PSDataCollection<T>)stream;

        psStream.DataAdded += (sender, e) =>
        {
            string? message = psStream[e.Index]?.ToString();
            AddOutput(message, streamType);
        };
    }
}
