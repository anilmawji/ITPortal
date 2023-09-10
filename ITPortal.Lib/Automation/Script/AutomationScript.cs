using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script.Parameter;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script;

[JsonDerivedType(typeof(PowerShellScript), typeDiscriminator: "powershell")]
public abstract class AutomationScript
{
    public string? FilePath { get; private set; }
    public string? FileName { get; private set; }
    public string[] Content { get; private set; }

    [JsonIgnore]
    public string ContentString { get; private set; }
    public List<ScriptParameter> Parameters { get; private set; }
    public ScriptLoadState LoadState { get; protected set; }

    public AutomationScript()
    {
        FileName = string.Empty;
        Content = Array.Empty<string>();
        ContentString = string.Empty;
        Parameters = new List<ScriptParameter>();
    }

    public AutomationScript(string filePath) : this()
    {
        LoadFromFile(filePath, true);
    }

    protected AutomationScript(string filePath, string fileName, string[] content, List<ScriptParameter> parameters)
    {
        FilePath = filePath;
        FileName = fileName;
        Content = content;
        ContentString = GetContentString();
        Parameters = parameters;
    }

    private string GetContentString()
    {
        return string.Join("\n", Content);
    }

    public abstract Task<ScriptExecutionState> InvokeAsync(string deviceName, ScriptOutputList scriptOutput, string cancellationMessage, CancellationToken cancellationToken);

    public abstract bool LoadParameters();

    public void AddParameter(string parameterName, Type parameterType, bool mandatory = false)
    {
        Parameters.Add(new ScriptParameter(parameterName, parameterType, mandatory));
    }

    public virtual bool LoadFromFile(string filePath, bool loadParameters)
    {
        ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));

        try
        {
            FileName = Path.GetFileName(filePath);
        }
        catch (ArgumentException)
        {
            return false;
        }
        FilePath = filePath;

        return DoLoadFromFile(FilePath, loadParameters);
    }

    private bool DoLoadFromFile(string filePath, bool loadParameters)
    {
        try
        {
            Content = File.ReadAllLines(filePath);
            ContentString = GetContentString();
            LoadState = ScriptLoadState.Success;

            return !loadParameters || LoadParameters();
        }
        catch (IOException)
        {
            LoadState = ScriptLoadState.Failed;

            return false;
        }
    }

    public virtual bool Refresh()
    {
        if (FilePath != null && LoadState == ScriptLoadState.Success)
        {
            return DoLoadFromFile(FilePath, true);
        }
        return false;
    }

    public void Unload()
    {
        LoadState = ScriptLoadState.Unloaded;
        FilePath = null;
        FileName = null;
        Content = Array.Empty<string>();
        ContentString = string.Empty;
    }

    public void ConvertParametersToArray()
    {
        foreach (ScriptParameter parameter in Parameters)
        {

        }
    }

    public bool IsUnloaded()
    {
        return LoadState == ScriptLoadState.Unloaded;
    }

    public bool IsLoaded()
    {
        return LoadState == ScriptLoadState.Success;
    }

    public bool LoadFailed()
    {
        return LoadState == ScriptLoadState.Failed;
    }
    
    public override string ToString()
    {
        return ContentString;
    }
}
