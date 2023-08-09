using ITPortal.Lib.Services.Automation.Output;

namespace ITPortal.Lib.Services.Automation.Script;

public abstract class AutomationScript
{
    public IScriptOutputStreamService OutputStreamService { get; set; }
    public string? Content { get; protected set; }
    public string? FilePath { get; protected set; }
    public string? Name { get; protected set; }
    public bool Loaded { get; protected set; }

    public AutomationScript(IScriptOutputStreamService outputStreamService)
    {
        OutputStreamService = outputStreamService;
    }

    public AutomationScript(IScriptOutputStreamService outputStreamService, string filePath) : this(outputStreamService)
    {
        Load(filePath);
    }

    public bool Load(string filePath)
    {
        FilePath = filePath;

        return LoadScript(FilePath);
    }

    public bool Refresh()
    {
        return FilePath != null && LoadScript(FilePath);
    }

    public abstract bool LoadScript(string filePath);

    public abstract Task InvokeAsync(CancellationToken cancellationToken, string cancellationMessage);
    
    public override string? ToString()
    {
        return Content;
    }
}
