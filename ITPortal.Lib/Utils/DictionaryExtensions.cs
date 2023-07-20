namespace ITPortal.Lib.Utils;

public static class DictionaryExtensions
{
    public static T Get<T>(this Dictionary<string, object> dictionary, string name)
    {
        return (T)dictionary[name];
    }
}
