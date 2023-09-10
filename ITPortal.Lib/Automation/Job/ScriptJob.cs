using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ITPortal.Lib.Automation.Job;

public sealed class ScriptJob : IDisposable
{
    // TODO : change to private set
    public AutomationScript Script { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; private set; }

    [JsonIgnore]
    public ScriptJobState State { get; private set; }
    public event EventHandler<ScriptJobState>? StateChanged;

    private CancellationTokenSource _cancellationTokenSource = new();

    [JsonConstructor]
    public ScriptJob(AutomationScript script, string name, string description, DateTime creationTime)
    {
        Script = script;
        Name = name;
        Description = description;
        CreationTime = creationTime;
    }

    public ScriptJob(AutomationScript script, string name) : this(script, name, string.Empty, DateTime.Now) { }

    public async Task<ScriptExecutionState> Run(string deviceName, ScriptJobResult result, string cancellationMessage)
    {
        SetState(ScriptJobState.Running);

        ScriptExecutionState executionResult = await Script.InvokeAsync(
            deviceName,
            result.ScriptOutput,
            cancellationMessage,
            _cancellationTokenSource.Token
        );
        SetState(ScriptJobState.Idle);

        result.InvokeExecutionResultReceived(executionResult);
        _cancellationTokenSource = new();

        return executionResult;
    }

    public void Cancel()
    {
        if (State == ScriptJobState.Running)
        {
            _cancellationTokenSource?.Cancel();
            SetState(ScriptJobState.Idle);
        }
    }

    private void SetState(ScriptJobState state)
    {
        State = state;
        StateChanged?.Invoke(this, State);
    }

    public static ScriptJob? TryLoadFromJsonFile(string filePath)
    {
        try
        {
            string jsonText = File.ReadAllText(filePath);
            ScriptJob? job = JsonSerializer.Deserialize(jsonText, ScriptJobContext.Default.ScriptJob);

            if (job == null || !Path.GetFileName(filePath).Contains(job.Name))
            {
                return null;
            }
            if (job.Script.FilePath != null)
            {
                job.Script.LoadFromFile(job.Script.FilePath, false);
            }
            return job;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string ToJsonString()
    {
        return JsonSerializer.Serialize(this, ScriptJobContext.Default.ScriptJob);
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
