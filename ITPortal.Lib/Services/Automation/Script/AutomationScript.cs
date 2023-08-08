using ITPortal.Lib.Services.Automation.Output;
using ITPortal.Lib.Services.Automation.Script.Parameter;

namespace ITPortal.Lib.Services.Automation.Script;

public abstract class AutomationScript
{
    public IScriptOutputStreamService OutputStreamService { get; set; }
    public ScriptParameterList Parameters { get; protected set; }
    public string? Content { get; protected set; }
    public string? FilePath { get; protected set; }
    public string? Name { get; protected set; }
    public bool Loaded { get; protected set; }

    public AutomationScript(IScriptOutputStreamService outputStreamService, ScriptParameterList parameters)
    {
        OutputStreamService = outputStreamService;
        Parameters = parameters;
    }

    public AutomationScript(IScriptOutputStreamService outputStreamService,
        ScriptParameterList parameters, string filePath) : this(outputStreamService, parameters)
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

    public abstract Task Invoke(CancellationToken cancellationToken);

        public override string? ToString()
    {
        return Content;
    }
}
