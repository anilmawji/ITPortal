using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

public sealed class ScriptJobResult : IDisposable
{
    public int Id { get; set; }
    public string? JobName { get; set; }
    public string? ScriptName { get; set; }
    public string? DeviceName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public ScriptOutputList ScriptOutput { get; set; }

    [JsonIgnore]
    public ScriptExecutionState ExecutionState { get; set; }
    public event EventHandler<ScriptExecutionState>? ExecutionResultReceived;

    [JsonConstructor]
    public ScriptJobResult(int id, string jobName, string scriptName, string deviceName, DateTime executionTime, ScriptOutputList scriptOutput)
    {
        Id = id;
        JobName = jobName;
        // We want to store a copy of the current script name and device name since they might change in the future
        ScriptName = scriptName;
        DeviceName = deviceName;
        ExecutionTime = executionTime;
        ScriptOutput = scriptOutput;
    }

    internal void InvokeExecutionResultReceived(ScriptExecutionState newState)
    {
        ExecutionResultReceived?.Invoke(this, newState);
    }

    public void Dispose()
    {
        ExecutionResultReceived.DisposeSubscriptions();
    }
}
