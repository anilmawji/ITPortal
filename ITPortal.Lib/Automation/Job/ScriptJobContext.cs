using ITPortal.Lib.Automation.Script.Parameter;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
[JsonSerializable(typeof(ScriptJob))]

// Support deserialization of 'object' type
[JsonSerializable(typeof(JsonElement))]

// Support serialization of script parameter types
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(List<string>))]
public partial class ScriptJobContext : JsonSerializerContext { }
