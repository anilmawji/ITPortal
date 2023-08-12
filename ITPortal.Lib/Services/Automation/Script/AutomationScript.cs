using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script.Parameter;

namespace ITPortal.Lib.Services.Automation.Script;

public abstract class AutomationScript
{
    public IScriptOutputStreamService OutputStreamService { get; set; }
    public string? FilePath { get; protected set; }
    public string? Name { get; protected set; }
    public string[]? Content { get; protected set; }
    public bool Loaded { get; protected set; }
    public ScriptParameterList? Parameters { get; protected set; }

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

        return LoadFromFile(FilePath);
    }

    public bool Refresh()
    {
        return FilePath != null && LoadFromFile(FilePath);
    }

    public abstract bool LoadFromFile(string filePath);

    public abstract Task InvokeAsync(CancellationToken cancellationToken, string cancellationMessage);

    public string? GetContentAsString()
    {
        return Content != null ? string.Join("\n", Content) : null;
    }
    
    public override string? ToString()
    {
        return GetContentAsString();
    }
}
