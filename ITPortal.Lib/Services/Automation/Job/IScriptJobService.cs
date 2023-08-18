using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

public interface IScriptJobService
{
    public Dictionary<int, ScriptJob> Jobs { get; set; }
    public ScriptJob NewJob(AutomationScript script, string deviceName);
    public void RegisterJob(ScriptJob job);
    public bool DeleteJob(int jobId);
}
