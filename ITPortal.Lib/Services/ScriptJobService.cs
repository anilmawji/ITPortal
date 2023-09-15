using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;

namespace ITPortal.Lib.Services;

public sealed class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    public Dictionary<string, ScriptJob> Jobs { get; private set; } = new();
    public Dictionary<int, ScriptJobResult> JobResults { get; private set; } = new();

    private int _nextResultId = 0;
    private int _lowestResultId = -1;

    public void AddJob(ScriptJob job)
    {
        Jobs.Add(job.Name, job);
    }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, ScriptOutputList scriptOutput)
    {
        ArgumentNullException.ThrowIfNull(job.Script.FileName, nameof(job.Script.FileName));

        Task<ScriptExecutionState> runJobTask = job.Run(
            deviceName,
            scriptOutput,
            ScriptOutputList.FormatAsSystemMessage("Script execution was cancelled")
        );
        ScriptJobResult jobResult = new(
            _nextResultId++,
            job.Name,
            job.Script.FileName,
            deviceName,
            DateTime.Now,
            scriptOutput,
            runJobTask
        );
        AddJobResult(jobResult);

        runJobTask.ContinueWith(task => jobResult.InvokeExecutionResultReceived(task.Result))
            .ConfigureAwait(false);

        return jobResult;
    }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName)
    {
        return RunJob(job, deviceName, job.Script.NewScriptOutputList());
    }
    
    private void AddJobResult(ScriptJobResult result)
    {
        if (JobResults.ContainsKey(result.Id)) return;

        JobResults.Add(result.Id, result);

        if (_nextResultId <= result.Id)
        {
            _nextResultId = result.Id + 1;
        }
        if (_lowestResultId > result.Id || _lowestResultId == -1)
        {
            _lowestResultId = result.Id;
        }
        // Cap the results list to store only the most recent results
        if (JobResults.Count > MaxResults)
        {
            JobResults.Remove(_lowestResultId);
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

    public List<ScriptJobResult> RemoveJobResults(ScriptJob job)
    {
        List<ScriptJobResult> results = new();

        foreach ((int id, ScriptJobResult result) in JobResults)
        {
            if (result.JobName == job.Name)
            {
                System.Diagnostics.Debug.WriteLine("Removing: " + result.Id);
                results.Add(result);
                JobResults.Remove(id);
            }
        }
        return results;
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

    public bool HasJobResult(int resultId)
    {
        return JobResults.GetValueOrDefault(resultId) != default(ScriptJobResult);
    }
}
