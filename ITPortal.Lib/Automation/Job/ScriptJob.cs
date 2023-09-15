using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

public sealed class ScriptJob : IDisposable
{
    public string Name { get; private set; }
    public string Description { get; set; }
    public AutomationScript Script { get; private set; }
    public DateTime CreationTime { get; private set; }

    [JsonIgnore]
    public ScriptJobState State { get; private set; }
    public event EventHandler<ScriptJobState>? StateChanged;

    private CancellationTokenSource _cancellationTokenSource = new();

    [JsonConstructor]
    public ScriptJob(string name, string description, AutomationScript script, DateTime creationTime)
    {
        Name = name;
        Description = description;
        Script = script;
        CreationTime = creationTime;
    }

    public ScriptJob(string name, AutomationScript script) : this(name, string.Empty, script, DateTime.Now) { }

    public async Task<ScriptExecutionState> Run(string deviceName, ScriptOutputList scriptOutput, string cancellationMessage)
    {
        SetState(ScriptJobState.Running);

        ScriptExecutionState executionResult = await Script.InvokeAsync(
            deviceName,
            scriptOutput,
            cancellationMessage,
            _cancellationTokenSource.Token
        );
        SetState(ScriptJobState.Idle);

        _cancellationTokenSource = new();

        return executionResult;
    }

    public void Cancel()
    {
        if (State == ScriptJobState.Running)
        {
            _cancellationTokenSource?.Cancel();
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

            if (job == null)
            {
                return null;
            }
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            if (fileName != job.Name)
            {
                return null;
            }
            if (job.Script.FilePath != null)
            {
                job.Script.LoadContent(job.Script.FilePath);
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

    public bool TrySetName(string name)
    {
        // TODO: string validation
        Name = name;
        return false;
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
