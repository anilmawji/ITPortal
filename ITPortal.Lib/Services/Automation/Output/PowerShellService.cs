using Microsoft.IdentityModel.Tokens;
using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation.Output
{
    public class PowerShellService : IOutputStreamService
    {
        public List<OutputMessage> Output { get; set; } = new();

        public event EventHandler<ScriptOutputChangedEventArgs>? OutputChanged;
        public bool HasOutputChangedHandler { get; set; }

        private OutputMessage? previousMessage;

        public void SubscribeToStream<T>(ICollection<T> stream, OutputStreamType streamType)
        {
            var psStream = (PSDataCollection<T>)stream;

            psStream.DataAdded += (sender, e) =>
            {
                string? message = psStream[e.Index]?.ToString();
                AddOutput(streamType, message);
            };
        }

        public void AddOutput(string message)
        {
            AddOutput(OutputStreamType.Standard, message);
        }

        public void AddOutput(OutputStreamType streamType, string? message)
        {
            if (message.IsNullOrEmpty() || OutputChanged == null) return;

            if (previousMessage?.Data == message)
            {
                Output.Last().Data += ".";
            }
            else
            {
                OutputMessage psMessage = new()
                {
                    Stream = streamType,
                    Data = message
                };

                Output.Add(psMessage);
                previousMessage = psMessage;
            }

            ScriptOutputChangedEventArgs args = new()
            {
                Output = Output,
                StreamType = streamType
            };
            OutputChanged?.Invoke(this, args);
        }
    }
}
