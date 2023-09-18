using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job.Result;

public sealed class ScriptJobResult : IDisposable
{
    public int Id { get; private set; }
    public string JobName { get; private set; }
    public string ScriptName { get; private set; }
    public string DeviceName { get; private set; }
    public DateTime ExecutionTime { get; private set; }
    public ScriptOutputList ScriptOutput { get; private set; }
    public ScriptExecutionState ExecutionState { get; private set; }
    public event EventHandler<ScriptExecutionState>? ExecutionResultReceived;

    [JsonIgnore]
    public Task<ScriptExecutionState>? RunJobTask { get; private set; }

    public ScriptJobResult(int id, string jobName, string scriptName, string deviceName, DateTime executionTime, ScriptOutputList scriptOutput)
    {
        Id = id;
        JobName = jobName;
        // We want to store a copy of the current script name and device name since they might change in the future
        ScriptName = scriptName;
        DeviceName = deviceName;
        ExecutionTime = executionTime;
        ScriptOutput = scriptOutput;
    }

    public ScriptJobResult(int id, string jobName, string scriptName, string deviceName, DateTime executionTime, ScriptOutputList scriptOutput,
        Task<ScriptExecutionState> runJobTask) : this(id, jobName, scriptName, deviceName, executionTime, scriptOutput)
    {
        RunJobTask = runJobTask;
    }

    [JsonConstructor]
    public ScriptJobResult(int id, string jobName, string scriptName, string deviceName, DateTime executionTime, ScriptOutputList scriptOutput,
        ScriptExecutionState executionState) : this(id, jobName, scriptName, deviceName, executionTime, scriptOutput)
    {
        ExecutionState = executionState;
    }

    internal void InvokeExecutionResultReceived(ScriptExecutionState newState)
    {
        ExecutionState = newState;
        ExecutionResultReceived?.Invoke(this, newState);
    }

    public static ScriptJobResult? LoadFromJsonFile(string filePath)
    {
        string jsonText = File.ReadAllText(filePath);
        ScriptJobResult? jobResult = JsonSerializer.Deserialize(jsonText, ScriptJobResultContext.Default.ScriptJobResult);

        if (jobResult == null)
        {
            return null;
        }
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        if (fileName != jobResult.Id.ToString())
        {
            return null;
        }
        return jobResult;
    }

    public string ToJsonString()
    {
        return JsonSerializer.Serialize(this, ScriptJobResultContext.Default.ScriptJobResult);
    }

    public void Dispose()
    {
        ExecutionResultReceived.DisposeSubscriptions();
    }
}
