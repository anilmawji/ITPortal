using ITPortal.Lib.Automation.Output;
using ITPortal.Lib.Automation.Script.Parameter;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script;

[JsonDerivedType(typeof(PowerShellScript), typeDiscriminator: "psScript")]
public abstract class AutomationScript
{
    public string? FilePath { get; protected set; }
    public string? FileName { get; protected set; }
    public string[]? Content { get; protected set; }

    [JsonIgnore]
    public string? ContentString { get; protected set; }
    public string DeviceName { get; set; } = "Localhost";
    public ScriptLoadState LoadState { get; protected set; }
    public List<ScriptParameter> Parameters { get; protected set; } = new();

    public AutomationScript() { }

    public AutomationScript(string filePath)
    {
        LoadFromFile(filePath);
    }

    public AutomationScript(string filePath, string deviceName) : this(filePath)
    {
        DeviceName = deviceName;
    }

    protected AutomationScript(string filePath, string fileName, string[] content, string deviceName, List<ScriptParameter> parameters)
    {
        FilePath = filePath;
        FileName = fileName;
        Content = content;
        ContentString = string.Join("\n", Content);
        DeviceName = deviceName;
        Parameters = parameters;
    }

    public abstract Task<ScriptExecutionState> InvokeAsync(string cancellationMessage, ScriptOutputList scriptOutput, CancellationToken cancellationToken);

    public abstract bool LoadParameters();

    public void AddParameter(string parameterName, Type parameterType, bool mandatory = false)
    {
        Parameters.Add(new ScriptParameter(parameterName, parameterType, mandatory));
    }

    public virtual bool LoadFromFile(string filePath)
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

        return DoLoadFromFile(FilePath);
    }

    private bool DoLoadFromFile(string filePath)
    {
        try
        {
            Content = File.ReadAllLines(filePath);
            ContentString = string.Join("\n", Content);
            LoadState = ScriptLoadState.Success;

            return LoadParameters();
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
            return DoLoadFromFile(FilePath);
        }
        return false;
    }

    public void Unload()
    {
        LoadState = ScriptLoadState.Unloaded;
        FilePath = null;
        FileName = null;
        Content = null;
        ContentString = null;
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
    
    public override string? ToString()
    {
        return ContentString;
    }
}
