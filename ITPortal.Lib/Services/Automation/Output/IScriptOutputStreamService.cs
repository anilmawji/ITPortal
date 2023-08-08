namespace ITPortal.Lib.Services.Automation.Output;

public interface IScriptOutputStreamService
{
    public List<ScriptOutputMessage> Output { get; set; }
    public event EventHandler<List<ScriptOutputMessage>>? OutputChanged;

    public void SubscribeToOutputStream<T>(ICollection<T> stream, ScriptStreamType streamType);

    public void AddOutput(string? message);

    public void AddOutput(ScriptStreamType streamType, string? message);
}
