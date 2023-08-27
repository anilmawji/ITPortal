using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;

namespace ITPortal.Lib.Automation.Job;

public sealed class ScriptJobResult
{
    public int Id { get; set; }
    public ScriptJob Job { get; set; }
    public string? ScriptName { get; set; }
    public string? DeviceName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public ScriptOutputList ScriptOutput { get; set; }
    public ScriptExecutionState ExecutionState { get; set; } = ScriptExecutionState.Running;
    public event EventHandler<ScriptExecutionState>? OnExecutionResultReceived = null;

    public ScriptJobResult(int id, ScriptJob job, string scriptName, string deviceName, DateTime executionTime, ScriptOutputList scriptOutput)
    {
        Id = id;
        Job = job;
        // We want to store a copy of the current script name and device name since they might change in the future
        ScriptName = scriptName;
        DeviceName = deviceName;
        ExecutionTime = executionTime;
        ScriptOutput = scriptOutput;
    }

    internal void InvokeOnExecutionResultReceived(ScriptExecutionState newState)
    {
        OnExecutionResultReceived?.Invoke(this, newState);
    }

    public bool DisposeOnExecutionResultReceivedEventSubscriptions()
    {
        return OnExecutionResultReceived.DisposeSubscriptions();
    }
}
