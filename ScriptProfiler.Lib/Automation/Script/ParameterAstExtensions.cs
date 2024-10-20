using System.Management.Automation.Language;

namespace ScriptProfiler.Lib.Automation.Script;

public static class ParameterAstExtensions
{
    public static bool HasParameter(this ParameterAst parameter, string value)
    {
        bool foundValue = false;

        foreach (AttributeBaseAst attribute in parameter.Attributes)
        {
            if (attribute.TypeName.ToString() == "Parameter")
            {
                string attributeText = attribute.ToString();

                // PS boolean parameters default to true if $true or $false isn't specified
                foundValue = attributeText.Contains(value) && !attributeText.Contains(value + "=$false");
            }
        }
        return foundValue;
    }

    public static bool HasType(this ParameterAst parameter, string typeName)
    {
        for (int i = 0; i < parameter.Attributes.Count; i++)
        {
            AttributeBaseAst attribute = parameter.Attributes[i];
            
            if (attribute.TypeName.ToString() == typeName)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AllowsEmptyAttribute(this ParameterAst parameter)
    {
        if (parameter.StaticType == typeof(string))
        {
            // Can't use typeof(System.Management.Automation.AllowEmptyStringAttribute).Name since the type name includes "Attribute"
            return parameter.HasType("AllowEmptyString");
        }

        if (parameter.StaticType.IsArray)
        {
            return parameter.HasType("AllowEmptyCollection");
        }

        return false;
    }

    public static ScriptParameter ToScriptParameter(this ParameterAst parameter)
    {
        return new ScriptParameter(
            parameter.Name.VariablePath.ToString(),
            parameter.StaticType,
            parameter.HasParameter("Mandatory"),
            parameter.AllowsEmptyAttribute()
        );
    }
}
