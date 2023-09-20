namespace ITPortal.Lib.Automation.Job.Result;

public sealed class ScriptJobResultList
{
    public int MaxResults { get; private set; }
    public int NextResultId { get; private set; } = 0;
    public int LowestResultId { get; private set; } = -1;

    private readonly Dictionary<int, ScriptJobResult> JobResults = new();

    public ScriptJobResultList(int maxResults)
    {
        MaxResults = maxResults;
    }

    public void Add(ScriptJobResult result)
    {
        JobResults.Add(NextResultId, result);

        if (LowestResultId > result.Id || LowestResultId == -1)
        {
            LowestResultId = result.Id;
        }
        // Cap the results list to store only the most recent results
        if (JobResults.Count > MaxResults)
        {
            JobResults.Remove(LowestResultId);
        }
        NextResultId++;
    }

    public bool TryAdd(ScriptJobResult result)
    {
        if (!HasResult(result.Id))
        {
            Add(result);

            return true;
        }
        return false;
    }

    public bool Remove(int id)
    {
        return JobResults.Remove(id);
    }

    public List<ScriptJobResult> Remove(ScriptJob job)
    {
        List<ScriptJobResult> results = new();

        foreach ((int id, ScriptJobResult result) in JobResults)
        {
            if (result.JobName == job.Name)
            {
                results.Add(result);
                JobResults.Remove(id);
            }
        }
        return results;
    }

    public bool HasResult(int resultId)
    {
        return JobResults.GetValueOrDefault(resultId) != default(ScriptJobResult);
    }

    public ScriptJobResult? TryGetResult(int id)
    {
        JobResults.TryGetValue(id, out ScriptJobResult? result);

        return result;
    }

    public IReadOnlyDictionary<int, ScriptJobResult> GetResults()
    {
        return JobResults.AsReadOnly();
    }
}
