using ScriptProfiler.Lib.Automation.Job;
using ScriptProfiler.Lib.Automation.Output;
using ScriptProfiler.Lib.Automation.Script;

namespace ScriptProfiler.Lib.Services;

public sealed class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    public ScriptJobCollection Jobs { get; private set; } = new();
    public ScriptJobResultCollection JobResults { get; private set; } = new(MaxResults);

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, ScriptOutputList scriptOutput, DateTime runDate = default)
    {
        ArgumentNullException.ThrowIfNull(job.Script.FileName, nameof(job.Script.FileName));

        Task<ScriptExecutionState> runJobTask = job.Run(
            deviceName,
            scriptOutput,
            ScriptOutputList.FormatAsSystemMessage("Script execution was cancelled"),
            runDate
        );

        ScriptJobResult jobResult = new(
            JobResults.GetNextResultId(),
            job.Name,
            job.Script.FileName,
            deviceName,
            DateTime.Now,
            scriptOutput,
            runJobTask
        );

        JobResults.Add(jobResult);
        runJobTask.ContinueWith(task => jobResult.InvokeExecutionResultReceived(task.Result));

        return jobResult;
    }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, DateTime runDate = default)
    {
        return RunJob(job, deviceName, job.Script.NewScriptOutputList(), runDate);
    }
}
