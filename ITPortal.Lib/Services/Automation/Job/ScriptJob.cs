using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJob
{
    public const string DefaultName = "Job";

    public string Name { get; set; }
    public AutomationScript Script { get; set; }
    public string Description { get; set; } = string.Empty;
    public ScriptJobState State { get; private set; }
    public DateTime CreationTime { get; private set; }

    public event EventHandler<ScriptJobState>? StateChanged;
    public bool HasStateChangedSubscriber;

    public ScriptJob(AutomationScript script, string name = DefaultName)
    {
        Script = script;
        Name = name;
        State = ScriptJobState.Idle;
        CreationTime = DateTime.Now;
    }

    public ScriptJob(AutomationScript script, string description, string name = DefaultName) : this(script, name)
    {
        Description = description;
    }

    public async Task Run(IOutputStreamService outputStream, CancellationToken cancellationToken)
    {
        SetState(ScriptJobState.Running);
        await Script.InvokeAsync("Script execution was cancelled", outputStream, cancellationToken);
        SetState(ScriptJobState.Idle);
    }

    public void SubscribeToStateChanged(EventHandler<ScriptJobState> handler)
    {
        StateChanged += handler;
        HasStateChangedSubscriber = true;
    }

    private void SetState(ScriptJobState state)
    {
        State = state;
        StateChanged?.Invoke(this, State);
    }
}
