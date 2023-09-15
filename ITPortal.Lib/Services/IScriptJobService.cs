using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Job.Result;
using ITPortal.Lib.Automation.Output;

namespace ITPortal.Lib.Services;

public interface IScriptJobService
{
    public ScriptJobList JobList { get; }
    public ScriptJobResultList JobResultList { get; }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, ScriptOutputList scriptOutput, DateTime runDate = default);
    public ScriptJobResult RunJob(ScriptJob job, string deviceName, DateTime runDate = default);
}
