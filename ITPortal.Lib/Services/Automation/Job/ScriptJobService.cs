using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJobService : IScriptJobService
{
    public Dictionary<int, ScriptJob> Jobs { get; set; } = new();

    private int _nextJobId = 0;

    // Creating a job MUST be followed by registering the job before creating a new one
    public ScriptJob NewJob(AutomationScript script, string deviceName)
    {
        return new ScriptJob(_nextJobId, script, deviceName);
    }

    public void RegisterJob(ScriptJob job)
    {
        Jobs.Add(job.Id, job);
        _nextJobId = GetNextJobId();
    }

    private int GetNextJobId()
    {
        for (int i = 0; i < Jobs.Count; i++)
        {
            if (!HasJob(i))
            {
                return i;
            }
        }
        return Jobs.Count;
    }

    public bool DeleteJob(int jobId)
    {
        bool removed = Jobs.Remove(jobId);

        if (jobId < _nextJobId)
        {
            _nextJobId = jobId;
        }
        return removed;
    }

    public ScriptJob? GetJobOrDefault(int jobId)
    {
        return Jobs.GetValueOrDefault(jobId);
    }

    public bool HasJob(int jobId)
    {
        return Jobs.GetValueOrDefault(jobId) != default(ScriptJob);
    }
}
