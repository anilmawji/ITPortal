using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script.Parameter;

public class ScriptParameterJsonConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
