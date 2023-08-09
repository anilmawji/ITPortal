using ITPortal.Lib.Services.Automation.Script;
using ITPortal.Lib.Utils;

namespace ITPortal.Lib.Services.Automation.JobTest;

public class ScriptJobService
{
    public int JobIdLength { get; set; }
    public Dictionary<string, ScriptJob> Jobs { get; set; }

    public ScriptJob AddNewJob(AutomationScript script, int jobIdLength)
    {
        string jobId = UniqueGuidGenerator.NewUniqueGuid(Jobs.Keys.ToList(), jobIdLength);
        ScriptJob job = new(jobId, script);

        Jobs.Add(jobId, job);

        return job;
    }
}
