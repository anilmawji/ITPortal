using System.Management.Automation;
using System.Management.Automation.Language;

namespace ITPortal.Lib.Services.Automation;

public class PSParameterList
{
    private readonly List<PSParameter> _parameters = new();

    public void Add(string parameterName, Type parameterType)
    {
        _parameters.Add(new PSParameter(parameterName, parameterType));
    }

    public void Add(ParameterAst parameter)
    {
        _parameters.Add(new PSParameter(parameter.Name.VariablePath.ToString(), parameter.StaticType));
    }

    public void Register(PowerShell shell)
    {
        foreach (var parameter in _parameters)
        {
            shell.AddParameter(parameter.Name, parameter.GetAsRequiredType());
        }
    }

    public List<PSParameter>.Enumerator GetEnumerator()
    {
        return _parameters.GetEnumerator();
    }
}
