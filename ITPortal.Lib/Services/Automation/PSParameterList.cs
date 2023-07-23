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
        _parameters.Add(new PSParameter(
            parameter.Name.VariablePath.ToString(),
            parameter.StaticType,
            IsMandatory(parameter)
        ));
    }

    public static bool IsMandatory(ParameterAst parameter)
    {
        bool mandatory = false;

        foreach (var attr in parameter.Attributes)
        {
            if (attr.TypeName.ToString() == "Parameter")
            {
                // TODO: Find way to extract property values from attribute
                mandatory = attr.ToString().Contains("Mandatory=$true");
                break;
            }
        }
        return mandatory;
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
