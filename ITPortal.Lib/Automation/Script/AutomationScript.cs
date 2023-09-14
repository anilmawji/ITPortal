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
    public ScriptLoadState ContentLoadState { get; private set; }

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

    public bool LoadContent(string filePath)
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

        return DoLoadContent(FilePath);
    }

    private bool DoLoadContent(string filePath)
    {
        try
        {
            Content = File.ReadAllLines(filePath);
            ContentString = GetContentString();
            ContentLoadState = ScriptLoadState.Success;

            return true;
        }
        catch (IOException)
        {
            ContentLoadState = ScriptLoadState.Failed;

            return false;
        }
    }

    public bool Refresh()
    {
        if (FilePath != null && ContentLoadState == ScriptLoadState.Success)
        {
            return DoLoadContent(FilePath) && LoadParameters();
        }
        return false;
    }

    public abstract bool LoadParameters();

    public void AddParameter(string parameterName, Type parameterType, bool mandatory = false)
    {
        Parameters.Add(new ScriptParameter(parameterName, parameterType, mandatory));
    }

    public abstract Task<ScriptExecutionState> InvokeAsync(string deviceName, ScriptOutputList scriptOutput, string cancellationMessage,
        CancellationToken cancellationToken);

    public void Unload()
    {
        FilePath = null;
        FileName = null;
        Content = Array.Empty<string>();
        ContentString = string.Empty;
        Parameters.Clear();
        ContentLoadState = ScriptLoadState.Unloaded;
    }

    private string GetContentString()
    {
        return string.Join("\n", Content);
    }

    public bool IsContentUnloaded()
    {
        return ContentLoadState == ScriptLoadState.Unloaded;
    }

    public bool IsContentLoaded()
    {
        return ContentLoadState == ScriptLoadState.Success;
    }

    public bool ContentFailedToLoad()
    {
        return ContentLoadState == ScriptLoadState.Failed;
    }
    
    public override string ToString()
    {
        return ContentString;
    }
}
