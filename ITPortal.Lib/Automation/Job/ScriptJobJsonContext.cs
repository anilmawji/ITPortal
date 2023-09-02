using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

[JsonSerializable(typeof(ScriptJob))]
[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class ScriptJobJsonContext : JsonSerializerContext
{

}
