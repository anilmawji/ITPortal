using ITPortal.Lib.Utilities.Extensions;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITPortal.Lib.Automation.Script.Parameter;

public sealed class ScriptParameter
{
    public const string UnknownValue = "Unknown";

    public string Name { get; private set; }
    public object Value { get; set; }
    public string TypeName { get; private set; }
    public bool Mandatory { get; private set; }

    public ScriptParameter(string name, Type type, bool mandatory = false)
    {
        Name = name;
        Value = GetDefaultValue(type);
        TypeName = type.AssemblyQualifiedName ?? string.Empty;
        Mandatory = mandatory;
    }

    [JsonConstructor]
    public ScriptParameter(string name, object value, string typeName, bool mandatory)
    {
        Name = name;
        Value = ((JsonElement)value).GetValue(Type.GetType(typeName)) ?? UnknownValue;
        TypeName = typeName;
        Mandatory = mandatory;
    }

    private static object GetDefaultValue(Type type)
    {
        // It is important to check IsValueType before calling GetUninitializedObject
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
        else
        {
            // Parameter value is a reference type
            if (type == typeof(string))
            {
                return string.Empty;
            }
            else if (type.IsArray)
            {
                if (type.GetElementType() == null)
                {
                    return UnknownValue;
                }
                // All arrays will be treated internally as lists of strings
                return new List<string>();
            }
        }
        return UnknownValue;
    }

    public bool IsType(Type type)
    {
        return TypeName == type.AssemblyQualifiedName;
    }

    public bool IsBinaryType()
    {
        return TypeName == typeof(bool).AssemblyQualifiedName || TypeName == typeof(SwitchParameter).AssemblyQualifiedName;
    }

    public override string? ToString()
    {
        return $"[{Name}, {Value}]";
    }
}

