using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

public sealed class ScriptJob : IDisposable
{
    public string Name { get; private set; }
    public string Description { get; set; }
    public AutomationScript Script { get; private set; }
    public DateTime CreationTime { get; private set; }

    [JsonIgnore]
    public ScriptJobState State { get; private set; }
    public event EventHandler<ScriptJobState>? StateChanged;

    private CancellationTokenSource _cancellationTokenSource = new();

    [JsonConstructor]
    public ScriptJob(string name, string description, AutomationScript script, DateTime creationTime)
    {
        Name = name;
        Description = description;
        Script = script;
        CreationTime = creationTime;
    }

    public ScriptJob(string name, string description, AutomationScript script) : this(name, description, script, DateTime.Now) { }

    public ScriptJob(string name, AutomationScript script) : this(name, string.Empty, script, DateTime.Now) { }

    // TODO: use runDate to determine when to run the job
    public async Task<ScriptExecutionState> Run(string deviceName, ScriptOutputList outputList,
        string cancellationMessage, DateTime runDate = default)
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

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }

    private void SetState(ScriptJobState state)
    {
        State = state;
        StateChanged?.Invoke(this, State);
    }

    public string ToJsonString()
    {
        return JsonSerializer.Serialize(this, ScriptJobContext.Default.ScriptJob);
    }

    public bool TrySetName(string name)
    {
        if (name.IsValidFileName())
        {
            Name = name;

            return true;
        }
        return false;
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
    }
}
