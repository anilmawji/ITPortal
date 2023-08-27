namespace ITPortal.Lib.Services.Automation.Output
{
    public class ScriptOutputStream
    {
        public List<OutputMessage> Output { get; set; } = new();

        public event EventHandler<ScriptOutputChangedEventArgs>? OnOutputChanged;

        private OutputMessage? previousMessage;
    }
}
