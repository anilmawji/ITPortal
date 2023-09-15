namespace ITPortal.Components.Models.Dialog
{
    public sealed class RunScriptJobDialogResult
    {
        public string DeviceName { get; set; }
        public bool ShouldRunJobNow { get; set; } = true;
        public bool ShouldViewJobResult { get; set; } = true;
        public DateTime RunDate { get; set; }

        public RunScriptJobDialogResult(string deviceName)
        {
            DeviceName = deviceName;
        }
    }
}
