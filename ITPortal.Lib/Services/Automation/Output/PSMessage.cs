namespace ITPortal.Lib.Services.Automation.Output;

public enum PSStream {
    Output,
    Information,
    Progress,
    Warning,
    Error
}

public class PSMessage
{
    public PSStream Stream;
    public string? Data;
}
