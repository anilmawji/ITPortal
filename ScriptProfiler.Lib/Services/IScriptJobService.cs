using ScriptProfiler.Lib.Automation.Job;
using ScriptProfiler.Lib.Automation.Output;

namespace ScriptProfiler.Lib.Services;

public interface IScriptJobService
{
    public ScriptJobCollection Jobs { get; }
    public ScriptJobResultCollection JobResults { get; }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, ScriptOutputList scriptOutput, DateTime runDate = default);

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, DateTime runDate = default);
}
