using System.Management.Automation.Language;

namespace ITPortal.Lib.Automation.Script.Parameter;

public static class ParameterAstExtensions
{
    public static bool IsMandatory(this ParameterAst parameter)
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
