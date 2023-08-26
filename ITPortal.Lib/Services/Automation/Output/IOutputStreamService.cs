namespace ITPortal.Lib.Services.Automation.Output;

public interface IOutputStreamService
{
    public List<OutputMessage> Output { get; set; }

    public event EventHandler<ScriptOutputChangedEventArgs>? OnOutputChanged;

    public void SubscribeToStream<V>(ICollection<V> stream, OutputStreamType streamType);

    public void AddOutput(string? message, OutputStreamType streamType = OutputStreamType.Standard);

    public void DisposeEventSubscriptions();
}
