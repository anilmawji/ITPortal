using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Output;

public class ScriptOutputMessage
{
    public ScriptOutputStreamType StreamType;
    public string? Data;

    public ScriptOutputMessage() { }

    [JsonConstructor]
    public ScriptOutputMessage(ScriptOutputStreamType streamType, string? data) => (StreamType, Data) = (streamType, data);
}
