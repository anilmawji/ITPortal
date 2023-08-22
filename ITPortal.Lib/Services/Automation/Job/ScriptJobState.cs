namespace ITPortal.Lib.Services.Automation.Job;

public enum ScriptJobState
{
    Idle,
    Scheduled,
    Running
}

public static class ScriptJobStateMethods
{
    public static string GetColor(this ScriptJobState state)
    {
        return state == ScriptJobState.Running ? "#0094FF" : "#747474";
    }
}