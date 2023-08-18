using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

// Easier to use a smaller cut and dry class for jobs instead of PSTaskJob or BackgroundJob from the PowerShell SDK
public class ScriptJob
{
    public int Id { get; set; }
    public AutomationScript Script { get; set; }
    public string DeviceName { get; set; }
    public ScriptJobState Status { get; private set; }
    public DateTime CreationTime { get; private set; }
    public DateTime? ExecutionTime { get; private set; }


    public ScriptJob(int id, AutomationScript script, string deviceName)
    {
        Id = id;
        Script = script;
        DeviceName = deviceName;
        CreationTime = DateTime.Now;
        Status = ScriptJobState.Idle;
    }

    public void Run(CancellationToken cancellationToken)
    {
        ExecutionTime = DateTime.Now;
        Status = ScriptJobState.Running;
        Script.InvokeAsync("Script execution was cancelled", cancellationToken);
    }
}
