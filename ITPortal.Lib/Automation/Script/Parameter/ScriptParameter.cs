using System.Management.Automation;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script.Parameter;

public sealed class ScriptParameter
{
    public string Name { get; set; }
    public object Value { get; set; }
    public string DesiredTypeName { get; private set; }
    public bool Mandatory { get; private set; }

    public ScriptParameter(string name, Type desiredType, bool mandatory = false)
    {
        Name = name;
        Value = GetDefaultValue(desiredType);
        DesiredTypeName = desiredType.Name;
        Mandatory = mandatory;
    }

    [JsonConstructor]
    public ScriptParameter(string name, object value, string desiredTypeName, bool mandatory)
    {
        Name = name;
        Value = value;
        DesiredTypeName = desiredTypeName;
        Mandatory = mandatory;
    }

    public static object GetDefaultValue(Type desiredType)
    {
        // It's important to check IsValueType before calling GetUninitializedObject
        // GetUninitializedObject is valid for reference types, but it will not return null
        if (desiredType.IsValueType)
        {
            if (desiredType == typeof(DateTime))
            {
                // Override default value for DateTime (1/1/1001)
                return DateTime.Today;
            }
            else if (desiredType == typeof(bool) || desiredType == typeof(SwitchParameter))
            {
                return false;
            }
            else if (desiredType == typeof(int) || desiredType == typeof(float) || desiredType == typeof(double))
            {
                // This is what Microsoft's codebase uses to get default values for value types at runtime
                return FormatterServices.GetUninitializedObject(desiredType);
            }
        }
        // Prepare default values for reference types
        else
        {
            if (desiredType == typeof(string))
            {
                return string.Empty;
            }
            else if (desiredType.IsArray)
            {
                if (desiredType.GetElementType() == null)
                {
                    return "Unknown";
                }
                // All arrays will be treated internally as lists of strings
                return new List<string>();
            }
        }
        return "Unknown";
    }

    public bool HasDesiredType(Type type)
    {
        return DesiredTypeName == type.Name;
    }

    public Type? GetValueType()
    {
        return Value?.GetType();
    }

    public override string? ToString()
    {
        return $"[{Name}, {Value}]";
    }
}

