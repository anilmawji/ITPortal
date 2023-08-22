using Microsoft.IdentityModel.Tokens;
using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation.Output
{
    public class PowerShellOutputStreamService : IOutputStreamService
    {
        public List<OutputMessage> Output { get; set; } = new();
        public Dictionary<StreamType, bool> OutputCompleted { get; set; } = new();
        public event EventHandler<List<OutputMessage>>? OutputChanged;
        public bool HasOutputChangedHandler { get; set; }

        private OutputMessage? previousMessage;

        public void SubscribeToStream<T>(ICollection<T> stream, StreamType streamType)
        {
            var psStream = (PSDataCollection<T>)stream;

            psStream.DataAdded += (sender, e) =>
            {
                string? message = psStream[e.Index]?.ToString();
                AddOutput(streamType, message);
            };
            psStream.Completed += (sender, e) =>
            {
                OutputCompleted[streamType] = true;
            };
        }

        public bool OnOutputChanged(EventHandler<List<OutputMessage>> outputChangedEvent)
        {
            if (OutputChanged == null)
            {
                OutputChanged += outputChangedEvent;
                HasOutputChangedHandler = true;

                return true;
            }
            return false;
        }

        public void AddOutput(string message)
        {
            AddOutput(StreamType.Standard, message);
        }

        public void AddOutput(StreamType streamType, string? message)
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
            // Execute callback function to update the UI
            OutputChanged?.Invoke(this, Output);
        }

        public void ResetStreamState()
        {
            foreach (StreamType streamType in OutputCompleted.Keys)
            {
                OutputCompleted[streamType] = false;
            }
        }

        public void ClearOutput()
        {
            Output.Clear();
            ResetStreamState();
        }

        public void Dispose()
        {
            foreach ()
        }
    }
}
