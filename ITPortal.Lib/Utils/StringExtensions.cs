namespace ITPortal.Lib.Utils;

public static class StringExtensions
{
    public static string EncodeBase64(this string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

        return Convert.ToBase64String(plainTextBytes);
    }

    public static string DecodeBase64(this string base64EncodedData)
    {
        if (string.IsNullOrEmpty(base64EncodedData))
        {
            return string.Empty;
        }
        byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
