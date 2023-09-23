namespace ITPortal.Lib.Utility;

public static class EnumHelper
{
    public static Dictionary<T, V> ToDictionary<T, V>(V defaultValue) where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .ToDictionary(e => e, e => defaultValue);
    }
}
