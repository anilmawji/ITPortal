using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Job;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    IncludeFields = true,
    WriteIndented = true)]
[JsonSerializable(typeof(ScriptJobResult))]
public partial class ScriptJobResultContext : JsonSerializerContext { }
