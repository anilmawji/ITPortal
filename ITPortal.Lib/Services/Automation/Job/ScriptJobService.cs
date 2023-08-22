using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    public Dictionary<string, ScriptJob> Jobs { get; set; } = new();
    public List<ScriptJobResult> Results { get; set; } = new();

    private int _nextResultId = 0;

    public void AddJob(ScriptJob job)
    {
        if (job.Name == ScriptJob.DefaultName)
        {
            int numJobsWithScriptName = GetNumJobsWithName(ScriptJob.DefaultName);

            if (numJobsWithScriptName > 0)
            {
                job.Name = $"Job ({numJobsWithScriptName})";
            }
        }
        Jobs.Add(job.Name, job);
    }

    public ScriptJobResult NewJobResult(ScriptJob job, IOutputStreamService outputStream)
    {
        ScriptJobResult result = new(_nextResultId++, job, DateTime.Now, outputStream);
        Results.Add(result);

        if (Results.Count > MaxResults)
        {
            Results.RemoveAt(Results.Count - 1);
        }
        return result;
    }

    public ScriptJobResult GetJobResult(int jobResultId)
    {
        return Results.ElementAt(jobResultId);
    }

    public int GetNumJobsWithName(string jobName)
    {
        int numJobs = 0;

        foreach (ScriptJob job in Jobs.Values)
        {
            if (job.Name == jobName)
            {
                numJobs++;
            }
        }
        return numJobs;
    }

    public ScriptJob? TryGetJob(string jobName)
    {
       return HasJob(jobName) ? Jobs[jobName] : null;
    }

    public bool HasJob(string jobName)
    {
        return Jobs.GetValueOrDefault(jobName) != default(ScriptJob);
    }
}
