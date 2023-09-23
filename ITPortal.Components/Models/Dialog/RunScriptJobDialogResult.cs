using ITPortal.Components.Utility;

namespace ITPortal.Components.Models.Dialog
{
    public sealed class RunScriptJobDialogResult
    {
        public string DeviceName { get; internal set; }
        public ScriptJobErrorAction ErrorAction { get; internal set; }
        public bool ShouldRunJobNow { get; internal set; } = true;
        public bool ShouldViewJobResult { get; internal set; } = true;
        public DateTime RunDate { get; internal set; }

        public RunScriptJobDialogResult(string deviceName, ScriptJobErrorAction errorAction)
        {
            DeviceName = deviceName;
            ErrorAction = errorAction;
        }
    }
}
