using Microsoft.IdentityModel.Tokens;
using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation.Output
{
    public class PowerShellOutputStreamService : IOutputStreamService
    {
        public List<OutputMessage> Output { get; set; } = new();
        public Dictionary<OutputStreamType, bool> OutputCompleted { get; set; } = new();

        public event EventHandler<List<OutputMessage>>? OutputChanged;
        public bool HasOutputChangedSubscriber { get; set; }

        private OutputMessage? previousMessage;

        public void SubscribeToStream<T>(ICollection<T> stream, OutputStreamType streamType)
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
            // Execute callback function to update the UI
            OutputChanged?.Invoke(this, Output);
        }

        public void ResetStreamCompletedState()
        {
            foreach (OutputStreamType streamType in OutputCompleted.Keys)
            {
                OutputCompleted[streamType] = false;
            }
        }

        public void ClearOutput()
        {
            Output.Clear();
            ResetStreamCompletedState();
        }
    }
}
