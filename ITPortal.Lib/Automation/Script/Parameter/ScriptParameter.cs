using System.Management.Automation;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script.Parameter;

public sealed class ScriptParameter
{
    public string Name { get; set; }
    public object Value { get; set; }
    public string TypeName { get; private set; }
    public bool Mandatory { get; private set; }

    public ScriptParameter(string name, Type type, bool mandatory = false)
    {
        Name = name;
        Value = GetDefaultValue(type);
        TypeName = type.Name;
        Mandatory = mandatory;
    }

    [JsonConstructor]
    public ScriptParameter(string name, object value, string typeName, bool mandatory)
    {
        Name = name;
        Value = value;
        TypeName = typeName;
        Mandatory = mandatory;
    }

    private static object GetDefaultValue(Type type)
    {
        // It's important to check IsValueType before calling GetUninitializedObject
        // GetUninitializedObject is valid for reference types, but it will not return null
        if (type.IsValueType)
        {
            if (type == typeof(DateTime))
            {
                // Override default value for DateTime (1/1/1001)
                return DateTime.Today;
            }
            else if (type == typeof(bool) || type == typeof(SwitchParameter))
            {
                return false;
            }
            else if (type == typeof(int) || type == typeof(float) || type == typeof(double))
            {
                // This is what Microsoft's codebase uses to get default values for value types at runtime
                return FormatterServices.GetUninitializedObject(type);
            }
        }
        // Prepare default values for reference types
        else
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }
            else if (type.IsArray)
            {
                if (type.GetElementType() == null)
                {
                    return "Unknown";
                }
                // All arrays will be treated internally as lists of strings
                return new List<string>();
            }
        }
        return "Unknown";
    }

    public bool IsType(Type type)
    {
        return TypeName == type.Name;
    }

    public override string? ToString()
    {
        return $"[{Name}, {Value}]";
    }
}

