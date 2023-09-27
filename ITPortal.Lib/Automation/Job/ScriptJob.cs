using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utility;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

public class ScriptJob : IScriptJob, IDisposable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public AutomationScript Script { get; private set; }
    public DateTime CreationTime { get; private set; }

    [JsonIgnore]
    public ScriptJobState State { get; private set; }
    public event EventHandler<ScriptJobState>? StateChanged;

    private CancellationTokenSource _cancellationTokenSource = new();

    public ScriptJob(string name, AutomationScript script) : this(name, string.Empty, script, DateTime.Now) { }

    public ScriptJob(string name, string description, AutomationScript script) : this(name, description, script, DateTime.Now) { }

    [JsonConstructor]
    public ScriptJob(string name, string description, AutomationScript script, DateTime creationTime)
    {
        Name = name;
        Description = description;
        Script = script;
        CreationTime = creationTime;

        if (Script.FilePath != null)
        {
            Script.LoadContent(Script.FilePath);
        }
    }

    // TODO: use runDate to determine when to run the job
    public async Task<ScriptExecutionState> Run(string deviceName, ScriptOutputList outputList, string cancellationMessage, DateTime runDate = default)
    {
        Task<ScriptExecutionState> runScript = Script.InvokeAsync(
            deviceName,
            outputList,
            cancellationMessage,
            _cancellationTokenSource.Token
        );

        SetState(ScriptJobState.Running);
        ScriptExecutionState executionResult = await runScript.ConfigureAwait(false);
        SetState(ScriptJobState.Idle);

        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();

        return executionResult;
    }

    private void SetState(ScriptJobState state)
    {
        State = state;
        StateChanged?.Invoke(this, State);
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }

    public bool IsIdle()
    {
        return State == ScriptJobState.Idle;
    }

    public bool IsScheduled()
    {
        return State == ScriptJobState.Scheduled;
    }

    public bool IsRunning()
    {
        return State == ScriptJobState.Running;
    }

    public void Dispose()
    {
        StateChanged.DisposeSubscriptions();
        GC.SuppressFinalize(this);
    }
}
