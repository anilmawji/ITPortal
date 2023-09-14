using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;

namespace ITPortal.Lib.Services;

public sealed class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    public Dictionary<string, ScriptJob> Jobs { get; private set; } = new();
    public Dictionary<int, ScriptJobResult> JobResults { get; private set; } = new();
    public Task<ScriptExecutionState>? LatestRunJobTask { get; private set; }

    private int _nextResultId = 0;
    private int _firstResultId = -1;

    public void AddJob(ScriptJob job)
    {
        Jobs.Add(job.Name, job);
    }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, ScriptOutputList scriptOutput)
    {
        ArgumentNullException.ThrowIfNull(job.Script.FileName, nameof(job.Script.FileName));

        ScriptJobResult jobResult = new(
            _nextResultId++,
            job.Name,
            job.Script.FileName,
            deviceName,
            DateTime.Now,
            scriptOutput
        );
        AddJobResult(jobResult);

        Task<ScriptExecutionState> jobTask = job.Run(deviceName, scriptOutput, ScriptOutputList.FormatAsSystemMessage("Script execution was cancelled"));
        LatestRunJobTask = jobTask;

        jobTask.ContinueWith(task => jobResult.InvokeExecutionResultReceived(task.Result))
            .ConfigureAwait(false);

        return jobResult;
    }

    private void AddJobResult(ScriptJobResult result)
    {
        if (result.Id >= _nextResultId)
        {
            _nextResultId = result.Id + 1;
        }
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
        if (job.TrySetName(newJobName))
        {
            AddJob(job);
            return true;
        }
        return false;
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

                if (JobResults.ContainsKey(resultId)) return;
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
