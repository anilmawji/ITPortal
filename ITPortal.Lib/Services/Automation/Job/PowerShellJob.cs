namespace ITPortal.Lib.Services.Automation.Job;

public class PowerShellJob
{
    public string Id { get; set; }
    public PowerShellScript Script { get; set; }
    public string Description { get; set; }

    public PowerShellJob(string id, PowerShellScript script, string description)
    {
        Id = id;
        Script = script;
        Description = description;
    }
}
