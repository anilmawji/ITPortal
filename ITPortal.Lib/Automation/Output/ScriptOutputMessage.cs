using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Output;

public sealed class ScriptOutputMessage
{
    public ScriptOutputStreamType StreamType;
    public string? Data;

    [JsonConstructor]
    public ScriptOutputMessage(ScriptOutputStreamType streamType, string? data) => (StreamType, Data) = (streamType, data);
}
