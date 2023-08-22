namespace ITPortal.Lib.Services.Automation.Output;

public interface IOutputStreamService
{
    public List<OutputMessage> Output { get; set; }
    public Dictionary<OutputStreamType, bool> OutputCompleted { get; set; }

    public event EventHandler<List<OutputMessage>>? OutputChanged;
    public bool HasOutputChangedSubscriber { get; set; }

    public void SubscribeToStream<V>(ICollection<V> stream, OutputStreamType streamType);

    public virtual void SubscribeToOutputChanged(EventHandler<List<OutputMessage>> outputChangedEvent)
    {
        OutputChanged += outputChangedEvent;
        HasOutputChangedSubscriber = true;
    }

    public void AddOutput(string message);

    public void AddOutput(OutputStreamType streamType, string? message);
}
