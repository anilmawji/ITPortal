using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script;
using ITPortal.Lib.Services.Event;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJob
{
    public AutomationScript Script { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public ScriptJobState State { get; private set; }
    public DateTime CreationTime { get; private set; }

    public event EventHandler<ScriptJobState>? OnStateChanged;
    public event EventHandler<ScriptExecutionState>? OnExecutionResultReceived = null;
    public event EventHandler<bool>? OnCancelled;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ScriptJob(AutomationScript script)
    {
        Script = script;

        _cancellationTokenSource.Token.Register(() => OnCancelled?.Invoke(this, State == ScriptJobState.Running));

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

    public async Task Run(IOutputStreamService outputStream, ScriptJobResult result)
    {
        SetState(ScriptJobState.Running);
        result.ExecutionState = await Script.InvokeAsync("Script execution was cancelled", outputStream, _cancellationTokenSource.Token);
        OnExecutionResultReceived?.Invoke(this, result.ExecutionState);
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

    public bool DisposeOnCancelledEventSubscriptions()
    {
        return OnCancelled.DisposeSubscriptions();
    }

    public bool DisposeOnExecutionResultReceivedEventSubscriptions()
    {
        return OnExecutionResultReceived.DisposeSubscriptions();
    }
}
