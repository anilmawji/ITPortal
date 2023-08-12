using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

public interface IScriptJobService
{
    public int JobIdLength { get; set; }
    public Dictionary<string, ScriptJob> Jobs { get; set; }

    public abstract ScriptJob NewJob(AutomationScript script, int jobIdLength);
}
