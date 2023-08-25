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

    public event EventHandler<ScriptJobState>? StateChanged;

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

    public async Task Run(IOutputStreamService outputStream, CancellationToken cancellationToken)
    {
        SetState(ScriptJobState.Running);
        await Script.InvokeAsync("Script execution was cancelled", outputStream, cancellationToken);
        SetState(ScriptJobState.Idle);
    }

    private void SetState(ScriptJobState state)
    {
        State = state;
        StateChanged?.Invoke(this, State);
    }

    public void DisposeEventSubscriptions()
    {
        if (StateChanged != null)
        {
            foreach (Delegate d in StateChanged.GetInvocationList())
            {
                StateChanged -= (EventHandler<ScriptJobState>)d;
            }
        }
    }
}
