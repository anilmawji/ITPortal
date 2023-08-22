namespace ITPortal.Lib.Services.Automation.Output;

public interface IOutputStreamService
{
    public List<OutputMessage> Output { get; set; }
    public Dictionary<StreamType, bool> OutputCompleted { get; set; }
    public event EventHandler<List<OutputMessage>>? OutputChanged;
    public bool HasOutputChangedHandler { get; set; }

    public void SubscribeToStream<V>(ICollection<V> stream, StreamType streamType);

    public void AddOutput(string message);

    public void AddOutput(StreamType streamType, string? message);

    public void Dispose();
}
