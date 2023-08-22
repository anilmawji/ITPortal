using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation.Job;

public struct ScriptJobResult
{
    public int Id { get; set; }
    public ScriptJob Job { get; set; }
    public string? ScriptName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public IOutputStreamService OutputStream { get; set; }
    public string DeviceName { get; set; }

    public ScriptJobResult(int id, ScriptJob job, DateTime executionTime, IOutputStreamService outputStream)
    {
        Id = id;
        Job = job;
        ExecutionTime = executionTime;
        OutputStream = outputStream;
        // We want to store a copy of these since they might change in the future
        ScriptName = job.Script.FileName;
        DeviceName = job.Script.DeviceName;
    }
}
