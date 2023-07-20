using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation;

public interface IOutputStreamService<T, V>
{
    public List<T> Output { get; set; }
    public event EventHandler<List<T>>? OutputChanged;

    public void SubscribeToPowerShellStream<K>(PSDataCollection<K> stream, V streamType);

    public void AddOutput(string? message);

    public void AddOutput(V streamType, string? message);
}
