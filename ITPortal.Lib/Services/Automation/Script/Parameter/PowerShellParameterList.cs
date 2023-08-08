using System.Management.Automation;
using System.Management.Automation.Language;

namespace ITPortal.Lib.Services.Automation.Script.Parameter;

// Extends from IEnumerable instead of List<PSParameter> to force caller to interact with the list through exposed methods only
public class PowershellParameterList : ScriptParameterList
{
    private readonly List<ScriptParameter> _parameters = new();

    public PowershellParameterList() { }

    public PowershellParameterList(IEnumerable<ParameterAst> parameters)
    {
        foreach (ParameterAst parameter in parameters)
        {
            Add(parameter);
        }
    }

    public void Add(ParameterAst parameter)
    {
        Add(parameter.Name.VariablePath.ToString(), parameter.StaticType, IsParameterMandatory(parameter));
    }

    private static bool IsParameterMandatory(ParameterAst parameter)
    {
        bool mandatory = false;

        foreach (AttributeBaseAst attribute in parameter.Attributes)
        {
            if (attribute.TypeName.ToString() == "Parameter")
            {
                // TODO: Find proper way to extract property values from attribute
                // Mandatory defaults to true if $true or $false isn't provided
                mandatory = attribute.ToString().Contains("Mandatory") && !attribute.ToString().Contains("Mandatory=$false");
                break;
            }
        }
        return mandatory;
    }

    public void Register(PowerShell shell)
    {
        foreach (ScriptParameter parameter in _parameters)
        {
            shell.AddParameter(parameter.Name, parameter.Value);
        }
    }
}
