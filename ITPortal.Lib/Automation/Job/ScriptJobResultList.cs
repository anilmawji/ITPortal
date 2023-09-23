namespace ITPortal.Lib.Automation.Job;

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

    public int GetNextResultId()
    {
        while (HasResult(NextResultId))
        {
            NextResultId++;
        }
        return NextResultId;
    }

    public void Add(ScriptJobResult result)
    {
        JobResults.Add(result.Id, result);

        // Update lowest result id
        if (LowestResultId > result.Id || LowestResultId == -1)
        {
            LowestResultId = result.Id;
        }
        // Cap the results list to store only the most recent results
        if (JobResults.Count > MaxResults)
        {
            Remove(LowestResultId);
        }
    }

    public bool Remove(int id)
    {
        if (JobResults.Remove(id))
        {
            if (id < NextResultId)
            {
                NextResultId = id;
            }
            return true;
        }
        return false;
    }

    public List<ScriptJobResult> Remove(ScriptJob job)
    {
        List<ScriptJobResult> removedResults = new();

        // Remove all job results associated with the job
        foreach ((int id, ScriptJobResult result) in JobResults)
        {
            if (result.JobName == job.Name)
            {
                removedResults.Add(result);
                Remove(id);
            }
        }
        return removedResults;
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
