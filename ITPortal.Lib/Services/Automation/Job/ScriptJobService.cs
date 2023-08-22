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
        // TODO: default job name should be the name of the script (jobName should be optional)
        // add (1) or (2), etc. to the end of the job name if it is not unique
        Jobs.Add(job.Name, job);
    }

    public ScriptJobResult NewJobResult(ScriptJob job, IOutputStreamService outputStream)
    {
        ScriptJobResult result = new(_nextResultId++, job, DateTime.Now, outputStream);
        AddScriptResult(result);

        return result;
    }

    private void AddScriptResult(ScriptJobResult result)
    {
        Results.Add(result);
        if (Results.Count > MaxResults)
        {
            Results.RemoveAt(Results.Count - 1);
        }
    }

    public ScriptJobResult GetJobResult(int jobResultId)
    {
        return Results.ElementAt(jobResultId);
    }

    public ScriptJob? TryGetJob(string jobName)
    {
       if (HasJob(jobName))
       {
            return Jobs[jobName];
       }
       return null;
    }

    public bool HasJob(string jobName)
    {
        return Jobs.GetValueOrDefault(jobName) != default(ScriptJob);
    }

    public bool DeleteJob(string jobName)
    {
        return Jobs.Remove(jobName);
    }
}