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
        ArgumentNullException.ThrowIfNull(job.Name, nameof(job.Name));

        Jobs.Add(job.Name, job);
    }

    public ScriptJobResult RunJob(ScriptJob job, IOutputStreamService outputStream, CancellationToken cancellationToken)
    {
        job.Run(outputStream, cancellationToken).ConfigureAwait(false);

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

    public ScriptJob? TryGetJob(string jobName)
    {
       return HasJob(jobName) ? Jobs[jobName] : null;
    }

    public bool HasJob(string jobName)
    {
        return Jobs.GetValueOrDefault(jobName) != default(ScriptJob);
    }
}
