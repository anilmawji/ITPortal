using Newtonsoft.Json.Linq;

namespace ITPortal.Lib.Utils;

public static class StringExtensions
{
    private static readonly char[] defaultSeparators = { ',', ' ' };

    public static string EncodeBase64(this string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string DecodeBase64(this string base64EncodedData)
    {
        if (string.IsNullOrEmpty(base64EncodedData))
        {
            return string.Empty;
        }
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static object? ConvertToArray(this string text, Type targetElementType)
    {
        return ConvertToArray(text, defaultSeparators, targetElementType);
    }

    public static object? ConvertToArray(this string text, char[] separators, Type targetElementType)
    {
        string[] valueArray = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        // No need for further operations if the desired array type is string
        if (targetElementType == typeof(string))
        {
            return valueArray;
        }

        try
        {
            object[] convertedArray = Array.ConvertAll(valueArray, elem => Convert.ChangeType(elem, targetElementType));

            return convertedArray;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
