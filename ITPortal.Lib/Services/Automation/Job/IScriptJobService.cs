using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

public interface IScriptJobService
{
    public int JobIdLength { get; set; }
    public Dictionary<string, ScriptJob> Jobs { get; set; }
    public ScriptJob NewJob(AutomationScript script, string deviceName, string jobDescription, int jobIdLength);

    public void RegisterJob(ScriptJob job);
}
