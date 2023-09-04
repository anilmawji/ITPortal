using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

public sealed class ScriptJob : IDisposable
{
    public AutomationScript Script { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;

    [JsonIgnore]
    public ScriptJobState State { get; private set; }
    public DateTime CreationTime { get; private set; }

    public event EventHandler<ScriptJobState>? StateChanged;

    private CancellationTokenSource? _cancellationTokenSource;

    public ScriptJob(AutomationScript script)
    {
        Script = script;
        CreationTime = DateTime.Now;
    }

    public ScriptJob(AutomationScript script, string name) : this(script)
    {
        Name = name;
    }

    [JsonConstructor]
    public ScriptJob(AutomationScript script, string name, string description) : this(script, name)
    {
        Description = description;
    }

    public async Task Run(ScriptJobResult result, string cancellationMessage)
    {
        _cancellationTokenSource = new();
        SetState(ScriptJobState.Running);

        ScriptExecutionState executionResult = await Script.InvokeAsync(
            cancellationMessage,
            result.ScriptOutput,
            _cancellationTokenSource.Token
        );
        result.InvokeExecutionResultReceived(executionResult);
        result.ExecutionState = executionResult;

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
