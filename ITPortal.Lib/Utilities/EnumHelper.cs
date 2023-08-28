namespace ITPortal.Lib.Utilities;

public static class EnumHelper
{
    public static Dictionary<T, bool> ToDictionary<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(e => e, e => false);
    }
}
