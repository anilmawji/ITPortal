using ITPortal.Lib.Automation.Output;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script;

[JsonDerivedType(typeof(PowerShellScript), typeDiscriminator: "powershell")]
public abstract class AutomationScript
{
    protected const string DefaultCancellationMessage = "Cancelled";

    public string? FilePath { get; private set; }
    public string? FileName { get; private set; }
    public string[]? FileContent { get; private set; }

    [JsonIgnore]
    public string? ContentString { get; private set; }
    public ScriptLoadState ContentLoadState { get; private set; }
    public List<ScriptParameter> Parameters { get; private set; }

    public AutomationScript()
    {
        Parameters = new List<ScriptParameter>();
    }

    public AutomationScript(string filePath, string[] fileContent, List<ScriptParameter> parameters)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(FilePath);
        FileContent = fileContent;
        ContentString = string.Join("\n", FileContent);
        Parameters = parameters;
        ContentLoadState = ScriptLoadState.Success;
    }

    public void LoadContent(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

        FilePath = filePath;
        FileName = Path.GetFileName(FilePath);

        DoLoadContent(FilePath);
    }

    private void DoLoadContent(string filePath)
    {
        FileContent = File.ReadAllLines(filePath);
        ContentString = string.Join("\n", FileContent);
        ContentLoadState = ScriptLoadState.Success;
    }

    public string[] Refresh()
    {
        if (FilePath != null && ContentLoadState == ScriptLoadState.Success)
        {
            DoLoadContent(FilePath);

            return LoadParameters();
        }

        return Array.Empty<string>();
    }

    public abstract string[] LoadParameters();

    public void AddParameter(string parameterName, Type parameterType, bool mandatory = false)
    {
        Parameters.Add(new ScriptParameter(parameterName, parameterType, mandatory));
    }

    public abstract ScriptOutputList NewScriptOutputList();

    public abstract Task<ScriptExecutionState> InvokeAsync(string deviceName, ScriptOutputList scriptOutput,
        string cancellationMessage = DefaultCancellationMessage, CancellationToken cancellationToken = default);

    public void Unload()
    {
        FilePath = null;
        FileName = null;
        FileContent = null;
        ContentString = null;
        Parameters.Clear();
        ContentLoadState = ScriptLoadState.Unloaded;
    }

    public bool IsContentUnloaded()
    {
        return ContentLoadState == ScriptLoadState.Unloaded;
    }

    public bool IsContentLoaded()
    {
        return ContentLoadState == ScriptLoadState.Success;
    }

    public bool ContentHasFailedToLoad()
    {
        return ContentLoadState == ScriptLoadState.Failed;
    }

    public override string ToString()
    {
        return ContentString ?? string.Empty;
    }
}
