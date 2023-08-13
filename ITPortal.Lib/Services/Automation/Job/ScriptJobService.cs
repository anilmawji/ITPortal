using ITPortal.Lib.Services.Automation.Script;
using ITPortal.Lib.Utils;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJobService : IScriptJobService
{
    public int JobIdLength { get; set; }
    public Dictionary<string, ScriptJob> Jobs { get; set; } = new();

    public ScriptJob NewJob(AutomationScript script, string deviceName, string jobDescription, int jobIdLength)
    {
        List<string> currentJobIds = Jobs.Keys.ToList();
        string jobId = UniqueGuidGenerator.NewUniqueGuid(currentJobIds, jobIdLength);

        return new ScriptJob(jobId, script, deviceName, jobDescription);
    }

    public void RegisterJob(ScriptJob job)
    {
        Jobs.Add(job.Id, job);
    }
}
