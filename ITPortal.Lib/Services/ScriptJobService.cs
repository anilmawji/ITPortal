using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Job.Result;
using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;

namespace ITPortal.Lib.Services;

public sealed class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    public Dictionary<string, ScriptJob> Jobs { get; private set; } = new();
    public ScriptJobResultList JobResultList { get; private set; } = new(MaxResults);

    public void AddJob(ScriptJob job)
    {
        Jobs.Add(job.Name, job);
    }

    public bool RemoveJob(string jobName)
    {
        return Jobs.Remove(jobName);
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
            JobResultList.NextResultId,
            job.Name,
            job.Script.FileName,
            deviceName,
            DateTime.Now,
            scriptOutput,
            runJobTask
        );
        JobResultList.Add(jobResult);

        runJobTask.ContinueWith(task => jobResult.InvokeExecutionResultReceived(task.Result))
            .ConfigureAwait(false);

        return jobResult;
    }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName)
    {
        return RunJob(job, deviceName, job.Script.NewScriptOutputList());
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

                if (JobResultList.HasResult(resultId)) return;
            }
            catch (Exception)
            {
                return;
            }
            ScriptJobResult? result = ScriptJobResult.TryLoadFromJsonFile(path);

            if (result != null)
            {
                JobResultList.Add(result);
            }
        }
    }

    public bool HasJob(string jobName)
    {
        return Jobs.GetValueOrDefault(jobName) != default(ScriptJob);
    }
}
