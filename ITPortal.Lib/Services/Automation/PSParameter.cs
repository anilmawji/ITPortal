using System.ComponentModel;

namespace ITPortal.Lib.Services.Automation;

public class PSParameter
{
    public string Name { get; set; }

    public string? Value { get; set; }

    // Used in place of generics since required type is discovered at runtime
    public Type RequiredType { get; set; }

    public PSParameter(string name, Type requiredType)
    {
        Name = name;
        RequiredType = requiredType;
    }

    public object? GetAsRequiredType()
    {
        if (Value == null) return null; 
        
        try
        {
            var typeConverter = TypeDescriptor.GetConverter(RequiredType);

            //System.Diagnostics.Debug.WriteLine(RequiredType.FullName + "    " + typeConverter.ConvertFromString(Value));

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
