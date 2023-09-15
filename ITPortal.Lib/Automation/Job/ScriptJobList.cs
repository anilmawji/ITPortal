namespace ITPortal.Lib.Automation.Job;

public class ScriptJobList
{
    public bool LoadedFromJson { get; private set; }

    private readonly Dictionary<string, ScriptJob> Jobs = new();

    public void Add(ScriptJob job)
    {
        Jobs.Add(job.Name, job);
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

    public void LoadScriptJobsFromJsonFiles(string folderPath)
    {
        LoadedFromJson = true;

        DirectoryInfo info = Directory.CreateDirectory(folderPath);
        // Folder has just been created; no jobs to load
        if (!info.Exists) return;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(folderPath);

        foreach (string path in filePaths)
        {
            if (HasJob(Path.GetFileNameWithoutExtension(path))) continue;

            ScriptJob? job = ScriptJob.TryLoadFromJsonFile(path);

            if (job != null)
            {
                Add(job);
            }
        }
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
