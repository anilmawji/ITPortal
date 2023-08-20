using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJobResult
{
    public int Id { get; set; }
    public ScriptJob Job { get; set; }
    public string? ScriptName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public IScriptOutputStreamService OutputStream { get; set; }
    public string DeviceName { get; set; }

    public ScriptJobResult(int id, ScriptJob job, DateTime executionTime, IScriptOutputStreamService outputStream)
    {
        Id = id;
        Job = job;
        ExecutionTime = executionTime;
        OutputStream = outputStream;
        // We want to store a copy of these since they might change in the script
        ScriptName = job.Script.FileName;
        DeviceName = job.Script.DeviceName;
    }
}
