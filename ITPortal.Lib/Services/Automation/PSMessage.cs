namespace ITPortal.Lib.Services.Automation;

public enum PSStream { Output, Information, Progress, Warning, Error }

public class PSMessage
{
    public PSStream Stream;
    public string? Data;
}
