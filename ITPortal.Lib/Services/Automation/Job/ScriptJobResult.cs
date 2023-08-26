using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation.Job;

public struct ScriptJobResult
{
    public int Id { get; set; }
    public string? ScriptName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public ScriptExecutionState ExecutionState { get; set; } = ScriptExecutionState.Running;
    public IOutputStreamService OutputStreamService { get; set; }
    public string DeviceName { get; set; }

    public ScriptJobResult(int id, string scriptName, string deviceName, DateTime executionTime, IOutputStreamService outputStreamService)
    {
        Id = id;
        // We want to store a copy of these since they might change in the future
        ScriptName = scriptName;
        DeviceName = deviceName;
        ExecutionTime = executionTime;
        OutputStreamService = outputStreamService;
    }
}
