using System.Collections;

namespace ScriptProfiler.Lib.Automation.Job;

public sealed class ScriptJobResultCollection : ICollection<ScriptJobResult>
{
    public int MaxResults { get; private set; }
    public int NextResultId { get; private set; } = 0;
    public int LowestResultId { get; private set; } = -1;
    public int Count => JobResults.Count;
    public bool IsReadOnly => false;

    private readonly Dictionary<int, ScriptJobResult> JobResults = [];

    public ScriptJobResultCollection(int maxResults)
    {
        MaxResults = maxResults;
    }

    public int GetNextResultId()
    {
        while (Contains(NextResultId))
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

    public bool Remove(ScriptJobResult result) => Remove(result.Id);

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
        List<ScriptJobResult> removedResults = [];

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

    public ScriptJobResult? GetResult(int id)
    {
        JobResults.TryGetValue(id, out ScriptJobResult? result);

        // TODO: replace with    return JobResults.GetValueOrDefault(id);
        return result;
    }

    public void Clear() => JobResults.Clear();

    public bool Contains(ScriptJobResult result) => JobResults.ContainsKey(result.Id);

    public bool Contains(int resultId) => JobResults.ContainsKey(resultId);

    public void CopyTo(ScriptJobResult[] array, int arrayIndex)
    {
        int index = arrayIndex;

        foreach (ScriptJobResult result in JobResults.Values)
        {
            array[index++] = result;
        }
    }

    public IEnumerator<ScriptJobResult> GetEnumerator() => JobResults.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => JobResults.Values.GetEnumerator();
}
