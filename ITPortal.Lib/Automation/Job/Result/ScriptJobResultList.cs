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

    public int GetNextResultId()
    {
        return 0;
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

    public void LoadScriptJobResultsFromJsonFiles(string folderPath)
    {
        DirectoryInfo info = Directory.CreateDirectory(folderPath);
        // Jobs folder has just been created; no jobs to load
        if (!info.Exists) return;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(folderPath);

        foreach (string path in filePaths)
        {
            try
            {
                int resultId = int.Parse(Path.GetFileNameWithoutExtension(path));

                if (HasResult(resultId)) return;
            }
            catch (Exception)
            {
                return;
            }
            ScriptJobResult? result = ScriptJobResult.TryLoadFromJsonFile(path);

            if (result != null)
            {
                Add(result);
            }
        }
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
