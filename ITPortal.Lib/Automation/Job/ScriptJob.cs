using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

public sealed class ScriptJob : IDisposable
{
    // TODO : change to private set
    public AutomationScript Script { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; private set; }

    [JsonIgnore]
    public ScriptJobState State { get; private set; }
    public event EventHandler<ScriptJobState>? StateChanged;

    private CancellationTokenSource _cancellationTokenSource = new();

    [JsonConstructor]
    public ScriptJob(AutomationScript script, string name, string description)
    {
        Script = script;
        Name = name;
        Description = description;
        CreationTime = DateTime.Now;
    }

    public ScriptJob(AutomationScript script, string name) : this(script, name, string.Empty) { }

    public async Task Run(string deviceName, ScriptJobResult result, string cancellationMessage)
    {
        SetState(ScriptJobState.Running);

        ScriptExecutionState executionResult = await Script.InvokeAsync(
            deviceName,
            result.ScriptOutput,
            cancellationMessage,
            _cancellationTokenSource.Token
        );
        result.InvokeExecutionResultReceived(executionResult);
        _cancellationTokenSource = new();

        SetState(ScriptJobState.Idle);
    }

    public bool TryCancel()
    {
        if (State == ScriptJobState.Running)
        {
            _cancellationTokenSource?.Cancel();

            return true;
        }
        return false;
    }

    private void SetState(ScriptJobState state)
    {
        State = state;
        StateChanged?.Invoke(this, State);
    }

    public void Dispose()
    {
        StateChanged.DisposeSubscriptions();
    }
}
