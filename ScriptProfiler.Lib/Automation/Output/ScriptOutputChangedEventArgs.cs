namespace ScriptProfiler.Lib.Automation.Output;

public class ScriptOutputChangedEventArgs : EventArgs
{
    public ScriptOutputStreamType StreamType { get; set; }
    public List<ScriptOutputMessage>? Output { get; set; }
}
