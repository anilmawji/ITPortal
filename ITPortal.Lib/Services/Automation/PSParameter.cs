namespace ITPortal.Lib.Services.Automation;

public class PSParameter
{
    public object? Value { get; private set; }

    // More effective than generics since this allows type to be set at runtime
    Type? RequiredType { get; set; }

    public PSParameter(Type requiredType)
    {
        RequiredType = requiredType;
    }

    public bool SetValue(object value)
    {
        if (value.GetType() == RequiredType)
        {
            Value = value;

            return true;
        }
        return false;
    }
}
