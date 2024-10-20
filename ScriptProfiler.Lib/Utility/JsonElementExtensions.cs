using System.Management.Automation;
using System.Text.Json;

namespace ScriptProfiler.Lib.Utility;

public static class JsonElementExtensions
{
    public static object? GetValue(this JsonElement element, Type? type)
    {
        if (type == null)
        {
            return null;
        }
        if (type.IsValueType)
        {
            if (type == typeof(DateTime))
            {
                return element.GetDateTime();
            }
            else if (type == typeof(bool) || type == typeof(SwitchParameter))
            {
                return element.GetBoolean();
            }
            else if (type == typeof(int))
            {
                return element.GetInt32();
            }
            else if (type == typeof(float))
            {
                return element.GetSingle();
            }
            else if (type == typeof(double))
            {
                return element.GetDouble();
            }
        }
        else
        {
            if (type == typeof(string))
            {
                return element.GetString();
            }
            else if (type.IsArray)
            {
                if (type.GetElementType() == null)
                {
                    return null;
                }
                return element.ToList();
            }
        }
        return null;
    }

    public static List<string> ToList(this JsonElement element)
    {
        List<string> list = new();

        foreach (JsonElement value in element.EnumerateArray())
        {
            string? valueString = value.GetString();
            if (valueString != null)
            {
                list.Add(valueString);
            }
        }
        return list;
    }

}
