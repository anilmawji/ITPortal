using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Output;

namespace ITPortal.Lib.Services;

public interface IScriptJobService
{
    public Dictionary<string, ScriptJob> Jobs { get; set; }
    public List<ScriptJobResult> JobResults { get; set; }

    public void AddJob(ScriptJob job);
    public string GenerateUniqueDefaultJobName();
    public ScriptJobResult RunJob(ScriptJob job, string deviceName, ScriptOutputList scriptOutput);
    public ScriptJobResult GetJobResult(int jobResultId);
    public bool HasJob(string jobName);
}
