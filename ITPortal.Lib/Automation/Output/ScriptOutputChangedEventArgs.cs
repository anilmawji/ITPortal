namespace ITPortal.Lib.Automation.Output;

public sealed class ScriptOutputChangedEventArgs : EventArgs
{
    public ScriptOutputStreamType StreamType { get; set; }
    public List<ScriptOutputMessage>? Output { get; set; }
}
