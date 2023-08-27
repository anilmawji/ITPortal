using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation.Job;

public interface IScriptJobService
{
    public Dictionary<string, ScriptJob> Jobs { get; set; }
    public List<ScriptJobResult> JobResults { get; set; }

    public void AddJob(ScriptJob job);
    public ScriptJobResult RunJob(ScriptJob job, ScriptOutputCollection outputCollection);
    public ScriptJobResult GetJobResult(int jobResultId);
    public ScriptJob? TryGetJob(string jobName);
    public bool HasJob(string jobName);
}
