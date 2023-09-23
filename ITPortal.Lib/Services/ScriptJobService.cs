using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;

namespace ITPortal.Lib.Services;

public sealed class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    public ScriptJobList JobList { get; private set; } = new();
    public ScriptJobResultList JobResultList { get; private set; } = new(MaxResults);

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
            JobResultList.GetNextResultId(),
            job.Name,
            job.Script.FileName,
            deviceName,
            DateTime.Now,
            scriptOutput,
            runJobTask
        );

        System.Diagnostics.Debug.WriteLine(jobResult.Id);

        JobResultList.Add(jobResult);
        runJobTask.ContinueWith(task => jobResult.InvokeExecutionResultReceived(task.Result));

        return jobResult;
    }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, DateTime runDate = default)
    {
        return RunJob(job, deviceName, job.Script.NewScriptOutputList(), runDate);
    }
}
