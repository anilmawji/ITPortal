using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script.Parameter;

namespace ITPortal.Lib.Services.Automation.Script;

public abstract class AutomationScript
{
    public IScriptOutputStreamService OutputStreamService { get; set; }
    public string? FilePath { get; protected set; }
    public string? FileName { get; protected set; }
    public ScriptLoadState LoadState { get; protected set; }
    public string[]? Content { get; protected set; }
    public ScriptParameterList? Parameters { get; protected set; }

    public AutomationScript(IScriptOutputStreamService outputStreamService)
    {
        OutputStreamService = outputStreamService;
    }

    public AutomationScript(IScriptOutputStreamService outputStreamService, string filePath) : this(outputStreamService)
    {
        LoadFromFile(filePath);
    }

    public abstract Task InvokeAsync(string cancellationMessage, CancellationToken cancellationToken);

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
            LoadState = ScriptLoadState.Succeeded;

            return true;
        }
        catch (IOException)
        {
            LoadState = ScriptLoadState.Failed;

            return false;
        }
    }

    public bool Refresh()
    {
        if (FilePath != null && LoadState == ScriptLoadState.Succeeded)
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
    }

    public bool IsUnloaded()
    {
        return LoadState == ScriptLoadState.Unloaded;
    }

    public bool IsLoaded()
    {
        return LoadState == ScriptLoadState.Succeeded;
    }

    public bool LoadFailed()
    {
        return LoadState == ScriptLoadState.Failed;
    }

    public string? GetContentAsString()
    {
        return Content != null ? string.Join("\n", Content) : null;
    }
    
    public override string? ToString()
    {
        return GetContentAsString();
    }
}
