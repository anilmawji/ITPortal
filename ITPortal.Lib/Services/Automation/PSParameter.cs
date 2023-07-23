using ITPortal.Lib.Utils;
using System.ComponentModel;
using System.Management.Automation;

namespace ITPortal.Lib.Services.Automation;

public class PSParameter
{
    public string Name { get; set; }
    public string? Value { get; set; }
    // Used in place of generics since required type is discovered at runtime
    public Type RequiredType { get; set; }
    public bool Mandatory { get; private set; }
    public int? Position { get; private set; }

    public PSParameter(string name, Type? requiredType, bool mandatory = false, int? position = null)
    {
        Name = name;
        RequiredType = requiredType ?? typeof(string);
        Mandatory = mandatory;
        Position = position;

        // Set default value to false if parameter is a bool
        if (RequiredType == typeof(bool))
        {
            Value = "False";
        // PowerShell switche type should behave the same as a bool
        } else if (RequiredType == typeof(SwitchParameter))
        {
            RequiredType = typeof(bool);
            Value = "False";
        }
    }

    public object? GetAsRequiredType()
    {
        if (Value == null)
        {
            return null;
        }

        if (RequiredType.IsArray)
        {
            Type? elementType = RequiredType.GetElementType();

            if (elementType == null)
            {
                return null;
            }
            return Value.ConvertToArray(elementType);
        }
        
        try
        {
            var typeConverter = TypeDescriptor.GetConverter(RequiredType);

            return typeConverter.ConvertFromString(Value);
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    public override string? ToString()
    {
        return $"[{Name}, {Value}]";
    }
}
