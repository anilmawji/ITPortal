namespace ITPortal.Lib.Services.Automation.Output;

public interface IOutputStreamService
{
    public List<OutputMessage> Output { get; set; }

    public event EventHandler<ScriptOutputChangedEventArgs>? OutputChanged;
    public bool HasOutputChangedHandler { get; set; }

    public void SubscribeToStream<V>(ICollection<V> stream, OutputStreamType streamType);

    public virtual void OnOutputChanged(EventHandler<ScriptOutputChangedEventArgs> outputChangedEvent)
    {
        OutputChanged += outputChangedEvent;
        HasOutputChangedHandler = true;
    }

    public void AddOutput(string? message, OutputStreamType streamType = OutputStreamType.Standard);
}
