using ITPortal.Lib.Services.Automation.Job;
using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script.Parameter;

namespace ITPortal.Lib.Services.Automation.Script;

public abstract class AutomationScript
{
    public string DeviceName { get; set; } = "Localhost";
    public string? FilePath { get; protected set; }
    public string? FileName { get; protected set; }
    public string[]? Content { get; protected set; }
    public string? ContentString { get; protected set; }
    public ScriptLoadState LoadState { get; protected set; }
    public ScriptParameterList Parameters { get; protected set; } = new();

    public AutomationScript() { }

    public AutomationScript(string filePath)
    {
        LoadFromFile(filePath);
    }

    public AutomationScript(string filePath, string deviceName) : this(filePath)
    {
        DeviceName = deviceName;
    }

    public abstract Task<ScriptExecutionResult> InvokeAsync(string cancellationMessage, IOutputStreamService outputStream, CancellationToken cancellationToken);

    public virtual bool LoadFromFile(string filePath)
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

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

            return true;
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
