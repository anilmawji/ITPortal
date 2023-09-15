using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Job.Result;
using ITPortal.Lib.Automation.Output;

namespace ITPortal.Lib.Services;

public interface IScriptJobService
{
    public Dictionary<string, ScriptJob> Jobs { get; }
    public ScriptJobResultList JobResultList { get; }

    public void AddJob(ScriptJob job);
    public bool RemoveJob(string jobName);
    public string GetUniqueDefaultJobName();
    public bool UpdateJobName(ScriptJob job, string newJobName);
    public ScriptJobResult RunJob(ScriptJob job, string deviceName, ScriptOutputList scriptOutput);
    public ScriptJobResult RunJob(ScriptJob job, string deviceName);
    public void LoadScriptJobs(string folderPath);
    public void LoadScriptJobResults(string folderPath);
    public bool HasJob(string jobName);
}
