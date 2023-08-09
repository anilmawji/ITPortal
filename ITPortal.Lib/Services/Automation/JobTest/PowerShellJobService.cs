namespace ITPortal.Lib.Services.Automation.JobTest;

public class PowerShellJobService : IScriptJobService
{
    public int JobIdLength { get; set; }
    public Dictionary<string, ScriptJob> Jobs { get; set; } = new();

    public PowerShellJobService(int jobIdLength)
    {
        JobIdLength = jobIdLength;
    }
}
