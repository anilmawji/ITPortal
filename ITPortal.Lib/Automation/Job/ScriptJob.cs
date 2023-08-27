using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;

namespace ITPortal.Lib.Automation.Job;

public sealed class ScriptJob
{
    public AutomationScript Script { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public ScriptJobState State { get; private set; }
    public DateTime CreationTime { get; private set; }
    public ScriptJobResult? LatestResult { get; private set; }

    public event EventHandler<ScriptJobState>? OnStateChanged;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ScriptJob(AutomationScript script)
    {
        Script = script;
    }

    public ScriptJob(AutomationScript script, string name) : this(script)
    {
        Script = script;
        Name = name;
        State = ScriptJobState.Idle;
        CreationTime = DateTime.Now;
    }

    public ScriptJob(AutomationScript script, string name, string description) : this(script, name)
    {
        Description = description;
    }

    public async Task Run(ScriptJobResult result)
    {
        LatestResult = result;
        SetState(ScriptJobState.Running);

        ScriptExecutionState executionResult = await Script.InvokeAsync("Script execution was cancelled", result.ScriptOutput, _cancellationTokenSource.Token);
        result.InvokeOnExecutionResultReceived(executionResult);
        result.ExecutionState = executionResult;

        SetState(ScriptJobState.Idle);
    }

    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
    }

    private void SetState(ScriptJobState state)
    {
        State = state;
        OnStateChanged?.Invoke(this, State);
    }

    public bool DisposeOnStateChangedEventSubscriptions()
    {
        return OnStateChanged.DisposeSubscriptions();
    }
}
