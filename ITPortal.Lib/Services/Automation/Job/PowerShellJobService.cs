namespace ITPortal.Lib.Services.Automation.Job;

public class PowerShellJobService : IScriptJobService
{
    public List<ScriptJob> Jobs { get; set; } = new();

}
