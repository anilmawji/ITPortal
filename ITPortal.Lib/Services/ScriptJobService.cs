using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Output;

namespace ITPortal.Lib.Services;

public sealed class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    public Dictionary<string, ScriptJob> Jobs { get; set; } = new();
    public List<ScriptJobResult> JobResults { get; set; } = new();

    private int _nextResultId = 0;

    public void AddJob(ScriptJob job)
    {
        Jobs.Add(job.Name, job);
    }

    public string GenerateUniqueDefaultJobName()
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
        Jobs.Add(newJobName, job);

        return true;
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

        JobResults.Add(result);
        // Cap the results list to store only the most recent results
        if (JobResults.Count > MaxResults)
        {
            JobResults.RemoveAt(0);
        }

        job.Run(deviceName, result, ScriptOutputList.FormatAsSystemMessage("Script execution was cancelled"))
            .ConfigureAwait(false);

        return result;
    }

    public bool LoadScriptJobFromJsonFile(string filePath)
    {
        string jsonText = File.ReadAllText(filePath);
        ScriptJob? job = ScriptJob.FromJsonString(jsonText);

        if (job == null || HasJob(job.Name))
        {
            return false;
        }
        if (job.Script.FilePath != null)
        {
            job.Script.LoadFromFile(job.Script.FilePath);
        }
        AddJob(job);

        return true;
    }

    public ScriptJobResult GetJobResult(int jobResultId)
    {
        return JobResults.ElementAt(jobResultId);
    }

    public bool HasJob(string jobName)
    {
        return Jobs.GetValueOrDefault(jobName) != default(ScriptJob);
    }
}
