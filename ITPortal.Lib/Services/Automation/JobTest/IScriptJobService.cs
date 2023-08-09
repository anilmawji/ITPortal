using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.JobTest;

public interface IScriptJobService
{
    public int JobIdLength { get; set; }
    public Dictionary<string, ScriptJob> Jobs { get; set; }

    public ScriptJob AddNewJob(AutomationScript script, int jobIdLength);
}
