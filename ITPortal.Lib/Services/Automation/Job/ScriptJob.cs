using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJob
{
    public string Name { get; set; }
    public AutomationScript Script { get; set; }
    public string Description { get; set; } = string.Empty;
    public ScriptJobState State { get; private set; }
    public DateTime CreationTime { get; private set; }
    public EventHandler<ScriptJobState>? OnStateChanged;
    public bool HasOnStateChangedHandler = false;

    public ScriptJob(string name, AutomationScript script)
    {
        Name = name;
        Script = script;
        State = ScriptJobState.Idle;
        CreationTime = DateTime.Now;
    }

    public ScriptJob(string name, AutomationScript script, string description) : this(name, script)
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
        OnStateChanged?.Invoke(this, State);
    }
}
