using System.Management.Automation;
using System.Runtime.Serialization;

namespace ITPortal.Lib.Automation.Script.Parameter;

public sealed class ScriptParameter
{
    public string Name { get; }
    public object? Value { get; set; }
    public bool Mandatory { get; }
    public string DesiredTypeName { get; private set; }

    public ScriptParameter(string name, Type desiredType, bool mandatory = false)
    {
        Name = name;
        Mandatory = mandatory;
        Value = GetDefaultValue(desiredType);
        DesiredTypeName = desiredType.Name;
    }

    public static object GetDefaultValue(Type desiredType)
    {
        // It's important to check IsValueType before calling GetUninitializedObject
        // GetUninitializedObject is valid for reference types, but it will not return null
        if (desiredType.IsValueType)
        {
            if (desiredType == typeof(DateTime))
            {
                // The default value for DateTime given by C# sucks (1/1/1001)
                return DateTime.Today;
            }
            else if (desiredType == typeof(SwitchParameter))
            {
                return false;
            }
            // This is what Microsoft's codebase uses to get default values for value types at runtime
            return FormatterServices.GetUninitializedObject(desiredType);
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

    public Type? GetValueType()
    {
        return Value?.GetType();
    }

    public override string? ToString()
    {
        return $"[{Name}, {Value}]";
    }
}

