using ITPortal.Lib.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Output;

[JsonDerivedType(typeof(PowerShellScriptOutputList), typeDiscriminator: "powershell")]
public abstract class ScriptOutputList : IDisposable
{
    public readonly Dictionary<ScriptOutputStreamType, int> StreamLineCounts = EnumHelper.ToDictionary<ScriptOutputStreamType, int>(0);
    public event EventHandler<ScriptOutputChangedEventArgs>? OutputChanged;

    private List<ScriptOutputMessage> Output { get; set; }

    [JsonIgnore]
    private ScriptOutputMessage? _previousMessage;

    public ScriptOutputList()
    {
        Output = new();
    }

    [JsonConstructor]
    public ScriptOutputList(List<ScriptOutputMessage> output) => (Output) = output;

    public abstract void SubscribeToOutputStream<T>(ICollection<T> stream, ScriptOutputStreamType streamType);

    public void Add(string? message, ScriptOutputStreamType streamType)
    {
        if (message.IsNullOrEmpty()) return;

        StreamLineCounts[streamType]++;

        if (_previousMessage?.Data == message)
        {
            Output.Last().Data += Environment.NewLine + ".";
        }
        else
        {
            ScriptOutputMessage outputMessage = new(streamType, message);
            Output.Add(outputMessage);
            _previousMessage = outputMessage;
        }
        OutputChanged?.Invoke(this, new ScriptOutputChangedEventArgs()
        {
            Output = Output,
            StreamType = streamType
        });
    }

    public static string FormatAsSystemMessage(string message)
    {
        return "[SYSTEM]: " + message;
    }

    public IReadOnlyList<ScriptOutputMessage> GetMessages()
    {
        return Output.AsReadOnly();
    }

    public IReadOnlyList<ScriptOutputMessage> GetMessagesFilteredByStream(ScriptOutputStreamType streamType)
    {
        return Output.Where(x => x.StreamType == streamType)
            .ToList()
            .AsReadOnly();
    }

    public void Clear()
    {
        Output.Clear();
    }

    public void Dispose()
    {
        OutputChanged.DisposeSubscriptions();
    }
}
