using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Utilities;

namespace ITPortal.Lib.Services.Automation.Job;

public sealed class ScriptJobResult
{
    public int Id { get; set; }
    public string? ScriptName { get; set; }
    public string DeviceName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public ScriptOutputCollection OutputCollection { get; set; }
    public ScriptExecutionState ExecutionState { get; set; } = ScriptExecutionState.Running;
    public event EventHandler<ScriptExecutionState>? OnExecutionResultReceived = null;

    public ScriptJobResult(int id, string scriptName, string deviceName, DateTime executionTime, ScriptOutputCollection outputCollection)
    {
        Id = id;
        // We want to store a copy of the current script name and device name since they might change in the future
        ScriptName = scriptName;
        DeviceName = deviceName;
        ExecutionTime = executionTime;
        OutputCollection = outputCollection;
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
