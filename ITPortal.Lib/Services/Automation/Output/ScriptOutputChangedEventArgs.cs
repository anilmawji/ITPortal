namespace ITPortal.Lib.Services.Automation.Output;

public class ScriptOutputChangedEventArgs : EventArgs
{
    public OutputStreamType StreamType { get; set; }
    public List<OutputMessage>? Output { get; set; }
}
