using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script;
using ITPortal.Lib.Utilities;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJob
{
    public AutomationScript Script { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public ScriptJobState State { get; private set; }
    public DateTime CreationTime { get; private set; }
    public ScriptJobResult? LatestResult { get; private set; }

    public event EventHandler<ScriptJobState>? OnStateChanged;
    public event EventHandler<ScriptExecutionState>? OnExecutionResultReceived = null;

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

    public async Task Run(IOutputStreamService outputStream, ScriptJobResult result)
    {
        LatestResult = result;
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

    public bool DisposeOnExecutionResultReceivedEventSubscriptions()
    {
        return OnExecutionResultReceived.DisposeSubscriptions();
    }
}
