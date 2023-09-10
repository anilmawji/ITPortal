using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Output;

namespace ITPortal.Lib.Services;

public sealed class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    public Dictionary<string, ScriptJob> Jobs { get; set; } = new();
    public Dictionary<int, ScriptJobResult> JobResults { get; set; } = new();

    private int _nextResultId = 0;
    private int _firstResultId = -1;

    public void AddJob(ScriptJob job)
    {
        Jobs.Add(job.Name, job);
    }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, ScriptOutputList scriptOutput)
    {
        ArgumentNullException.ThrowIfNull(job.Script.FileName, nameof(job.Script.FileName));

        ScriptJobResult result = new(
            _nextResultId++,
            job.Name,
            job.Script.FileName,
            deviceName,
            DateTime.Now,
            scriptOutput
        );
        AddJobResult(result);

        job.Run(deviceName, result, ScriptOutputList.FormatAsSystemMessage("Script execution was cancelled"))
            .ConfigureAwait(false);

        return result;
    }

    private void AddJobResult(ScriptJobResult result)
    {
        JobResults.Add(result.Id, result);
        if (result.Id < _firstResultId || _firstResultId == -1)
        {
            _firstResultId = result.Id;
        }
        if (JobResults.Count > MaxResults)
        {
            // Cap the results list to store only the most recent results
            JobResults.Remove(_firstResultId);
        }
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
        if (!Jobs.Remove(job.Name))
        {
            return false;
        }
        job.Name = newJobName;
        AddJob(job);

        return true;
    }

    public void LoadScriptJobs(string folderPath)
    {
        DirectoryInfo info = Directory.CreateDirectory(folderPath);
        // Jobs folder has just been created; no jobs to load
        if (!info.Exists) return;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(folderPath);

        foreach (string path in filePaths)
        {
            if (HasJob(Path.GetFileNameWithoutExtension(path))) continue;

            ScriptJob? job = ScriptJob.TryLoadFromJsonFile(path);

            if (job != null)
            {
                AddJob(job);
            }
        }
    }

    public void LoadScriptJobResults(string folderPath)
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

                if (JobResults.ContainsKey(resultId))
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
            ScriptJobResult? result = ScriptJobResult.TryLoadFromJsonFile(path);

            if (result != null)
            {
                AddJobResult(result);
            }
        }
    }

    public bool HasJob(string jobName)
    {
        return Jobs.GetValueOrDefault(jobName) != default(ScriptJob);
    }
}
