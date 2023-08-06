using System.Management.Automation.Language;

namespace ITPortal.Lib.Services.Automation.Parameter;

public static class ParameterAstExtensions
{
    public static bool IsMandatory(this ParameterAst parameter)
    {
        bool mandatory = false;

        foreach (var attribute in parameter.Attributes)
        {
            if (attribute.TypeName.ToString() == "Parameter")
            {
                // TODO: Find proper way to extract property values from attribute
                mandatory = attribute.ToString().Contains("Mandatory") && !attribute.ToString().Contains("Mandatory=$false");
                break;
            }
        }
        return mandatory;
    }
}
