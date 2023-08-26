using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJob
{
    public AutomationScript Script { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public ScriptJobState State { get; private set; }
    public DateTime CreationTime { get; private set; }

    public event EventHandler<ScriptJobState>? OnStateChanged;
    public event EventHandler? OnCancelled;

    private readonly CancellationTokenSource _cancellationTokenSource;

    public ScriptJob(AutomationScript script)
    {
        Script = script;

        _cancellationTokenSource = new();
        _cancellationTokenSource.Token.Register(() => OnCancelled?.Invoke(this, EventArgs.Empty));

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

    public void DisposeEventSubscriptions()
    {
        if (OnStateChanged != null)
        {
            foreach (Delegate d in OnStateChanged.GetInvocationList())
            {
                OnStateChanged -= (EventHandler<ScriptJobState>)d;
            }
        }
        if (OnCancelled != null)
        {
            foreach (Delegate d in OnCancelled.GetInvocationList())
            {
                OnCancelled -= (EventHandler)d;
            }
        }
    }
}
