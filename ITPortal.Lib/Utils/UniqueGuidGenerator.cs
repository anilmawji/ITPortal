namespace ITPortal.Lib.Utils;

public static class UniqueGuidGenerator
{
    public static string NewUniqueGuid(List<string> ids, int guidLength)
    {
        string newId = NewGuid(guidLength);

        while (ids.Contains(newId))
        {
            newId = NewGuid(guidLength);
        }
        return newId;
    }

    private static string NewGuid(int guidLength)
    {
        return Guid.NewGuid().ToString()[guidLength..];
    }
}
