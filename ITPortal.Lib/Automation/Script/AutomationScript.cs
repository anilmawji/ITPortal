using ITPortal.Lib.Automation.Output;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script;

[JsonDerivedType(typeof(PowerShellScript), typeDiscriminator: "powershell")]
public abstract class AutomationScript
{
    protected const string DefaultCancellationMessage = "Cancelled";

    public string? FilePath { get; private set; }
    public string? FileName { get; private set; }
    public string[] Content { get; private set; }

    [JsonIgnore]
    public string ContentString { get; private set; }
    public ScriptLoadState ContentLoadState { get; private set; }
    public List<ScriptParameter> Parameters { get; private set; }

    public AutomationScript()
    {
        FileName = string.Empty;
        Content = Array.Empty<string>();
        ContentString = string.Empty;
        Parameters = new List<ScriptParameter>();
    }

    public AutomationScript(string filePath) : this()
    {
        LoadContent(filePath);
        LoadParameters();
    }

    public AutomationScript(string filePath, string fileName, string[] content, List<ScriptParameter> parameters)
    {
        FilePath = filePath;
        FileName = fileName;
        Content = content;
        ContentString = GetContentString();
        Parameters = parameters;
        ContentLoadState = ScriptLoadState.Success;
    }

    private string GetContentString()
    {
        return string.Join("\n", Content);
    }

    public void LoadContent(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));
        
        FileName = Path.GetFileName(filePath);
        FilePath = filePath;

        DoLoadContent(FilePath);
    }

    private void DoLoadContent(string filePath)
    {
        Content = File.ReadAllLines(filePath);
        ContentString = GetContentString();
        ContentLoadState = ScriptLoadState.Success;
    }

    public bool TryRefresh()
    {
        if (FilePath != null && ContentLoadState == ScriptLoadState.Success)
        {
            DoLoadContent(FilePath);
            LoadParameters();

            return true;
        }
        return false;
    }

    public abstract bool LoadParameters();

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
        Content = Array.Empty<string>();
        ContentString = string.Empty;
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
        return ContentString;
    }
}
