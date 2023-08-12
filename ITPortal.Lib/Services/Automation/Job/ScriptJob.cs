using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

// Easier to use a smaller cut and dry class for jobs instead of PSTaskJob or BackgroundJob from the PowerShell SDK
public class ScriptJob
{
    public string Id { get; set; }
    public AutomationScript Script { get; set; }
    public string? DeviceName { get; set; }
    public string? Description { get; set; }
    public ScriptJobState State { get; private set; }
    public DateTime? ExecutionTime { get; private set; }

    public ScriptJob(string id, AutomationScript script)
    {
        Id = id;
        Script = script;
    }

    public ScriptJob(string id, AutomationScript script, string description) : this(id, script)
    {
        Description = description;
    }
}
