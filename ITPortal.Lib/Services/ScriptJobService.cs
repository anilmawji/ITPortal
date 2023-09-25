using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;

namespace ITPortal.Lib.Services;

public sealed class ScriptJobService : IScriptJobService
{
    public const int MaxResults = 50;

    //TODO: remove jobList from here, move it to caller code
    public ScriptJobCollection Jobs { get; private set; } = new();
    public ScriptJobResultCollection JobResults { get; private set; } = new(MaxResults);
    public bool JobsLoaded { get; private set; }
    public bool JobResultsLoaded { get; private set; }



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

        System.Diagnostics.Debug.WriteLine(jobResult.Id);

        JobResults.Add(jobResult);
        runJobTask.ContinueWith(task => jobResult.InvokeExecutionResultReceived(task.Result));

        return jobResult;
    }

    public ScriptJobResult RunJob(ScriptJob job, string deviceName, DateTime runDate = default)
    {
        return RunJob(job, deviceName, job.Script.NewScriptOutputList(), runDate);
    }
}
