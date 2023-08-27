using ITPortal.Lib.Script.Parameter;
using System.Management.Automation.Language;

namespace ITPortal.Lib.Automation.Script.Parameter;

public sealed class PowershellParameterList : ScriptParameterList
{
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
                string attributeText = attribute.ToString();
                // TODO: Find proper way to extract property values from attribute
                // Mandatory defaults to true if $true or $false isn't provided
                mandatory = attributeText.Contains("Mandatory") && !attributeText.Contains("Mandatory=$false");
                break;
            }
        }
        return mandatory;
    }
}
