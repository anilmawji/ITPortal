using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation.Output;

public sealed class PowerShellOutputList : ScriptOutputList
{
    public override void SubscribeToOutputStream<T>(ICollection<T> stream, ScriptOutputStreamType streamType)
    {
        var psStream = (PSDataCollection<T>)stream;

        psStream.DataAdded += (sender, e) =>
        {
            string? message = psStream[e.Index]?.ToString();
            Add(message, streamType);
        };
    }
}
