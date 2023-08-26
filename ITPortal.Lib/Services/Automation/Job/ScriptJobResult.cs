using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJobResult
{
    public int Id { get; set; }
    public string? ScriptName { get; set; }
    public string DeviceName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public IOutputStreamService OutputStreamService { get; set; }
    public ScriptExecutionState ExecutionState { get; set; } = ScriptExecutionState.Running;

    public ScriptJobResult(int id, string scriptName, string deviceName, DateTime executionTime, IOutputStreamService outputStreamService)
    {
        Id = id;
        // We want to store a copy of the current script name and device name since they might change in the future
        ScriptName = scriptName;
        DeviceName = deviceName;
        ExecutionTime = executionTime;
        OutputStreamService = outputStreamService;
    }
}
