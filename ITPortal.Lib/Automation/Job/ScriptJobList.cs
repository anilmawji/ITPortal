namespace ITPortal.Lib.Automation.Job;

public class ScriptJobList
{
    public bool LoadedFromJson { get; private set; }

    private readonly Dictionary<string, ScriptJob> Jobs = new();

    public void Add(ScriptJob job)
    {
        Jobs.Add(job.Name, job);
    }

    public bool TryAdd(ScriptJob job)
    {
        if (!HasJob(job.Name))
        {
            Jobs.Add(job.Name, job);

            return true;
        }
        return false;
    }

    public bool Remove(string jobName)
    {
        return Jobs.Remove(jobName);
    }

    public string GetUniqueDefaultJobName()
    {
        string name = $"Job({Jobs.Count})";

        while (HasJob(name))
        {
            name += "(1)";
        }
        return name;
    }

    public bool UpdateJobName(ScriptJob job, string newJobName)
    {
        if (!Remove(job.Name))
        {
            return false;
        }
        if (job.TrySetName(newJobName))
        {
            Add(job);
            return true;
        }
        return false;
    }

    public bool HasJob(string jobName)
    {
        return Jobs.GetValueOrDefault(jobName) != default(ScriptJob);
    }

    public ScriptJob? TryGetJob(string jobName)
    {
        Jobs.TryGetValue(jobName, out ScriptJob? result);

        return result;
    }

    public IReadOnlyDictionary<string, ScriptJob> GetJobs()
    {
        return Jobs.AsReadOnly();
    }
}
