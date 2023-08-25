using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation.Job;

public interface IScriptJobService
{
    public Dictionary<string, ScriptJob> Jobs { get; set; }
    List<ScriptJobResult> Results { get; set; }

    public void AddJob(ScriptJob job);
    public ScriptJobResult RunJob(ScriptJob job, IOutputStreamService outputStreamService, CancellationToken cancellationToken);
    public ScriptJobResult GetJobResult(int jobResultId);
    public ScriptJob? TryGetJob(string jobName);
    public bool HasJob(string jobName);
}
