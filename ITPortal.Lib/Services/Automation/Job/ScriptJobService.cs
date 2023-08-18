using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJobService : IScriptJobService
{
    public Dictionary<int, ScriptJob> Jobs { get; set; } = new();
    private int nextJobId = 0;

    public ScriptJob NewJob(AutomationScript script, string deviceName)
    {
        return new ScriptJob(nextJobId, script, deviceName);
    }

    public void RegisterJob(ScriptJob job)
    {
        Jobs.Add(job.Id, job);
        nextJobId = GetNextJobId();
    }

    private int GetNextJobId()
    {
        for (int i = 0; i < Jobs.Count; i++)
        {
            if (!Jobs.ContainsKey(i))
            {
                return i;
            }
        }
        return Jobs.Count;
    }

    public bool DeleteJob(int jobId)
    {
        bool removed = Jobs.Remove(jobId);

        if (jobId < nextJobId)
        {
            nextJobId = jobId;
        }
        return removed;
    }
}
