using ScriptProfiler.Lib.Automation.Output;
using ScriptProfiler.Lib.Automation.Script;

namespace ScriptProfiler.Lib.Automation.Job;

public interface IScriptJob
{
    public string Name { get; }
    public string Description { get; }
    public AutomationScript Script { get; }
    public DateTime CreationTime { get; }
    public ScriptJobState State { get; }
    public event EventHandler<ScriptJobState>? StateChanged;

    public Task<ScriptExecutionState> Run(string deviceName, ScriptOutputList outputList, string cancellationMessage, DateTime runDate = default);

    public void Cancel();

    public bool IsIdle();

    public bool IsScheduled();

    public bool IsRunning();

    public void Dispose();
}
