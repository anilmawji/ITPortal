using System.Management.Automation;

namespace ITPortal.Lib.Automation.Output;

public sealed class PowerShellScriptOutputList : ScriptOutputList
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
