using ITPortal.Lib.Automation.Script.Parameter;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Script.Parameter;

[JsonDerivedType(typeof(PowerShellParameterList), typeDiscriminator: "psList")]
public class ScriptParameterList
{
    public List<ScriptParameter> Parameters { get; private set; } = new();

    public ScriptParameterList() { }

    [JsonConstructor]
    public ScriptParameterList(List<ScriptParameter> parameters)
    {
        Parameters = parameters;
    }

    public void Add(string parameterName, Type parameterType, bool mandatory = false)
    {
        Parameters.Add(new ScriptParameter(parameterName, parameterType, mandatory));
    }

    public IEnumerator<ScriptParameter> GetEnumerator()
    {
        return Parameters.GetEnumerator();
    }
}
