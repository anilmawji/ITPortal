using ITPortal.Lib.Services.Automation.Script;

namespace ITPortal.Lib.Services.Automation.ScriptJob;

public class ScriptJob
{
    public string Id { get; set; }
    public AutomationScript? Script { get; set; }
    public string? Description { get; set; }

    public ScriptJob(string id)
    {
        Id = id;
    }
}
