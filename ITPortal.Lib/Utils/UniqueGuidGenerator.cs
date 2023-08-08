namespace ITPortal.Lib.Utils;

public class UniqueGuidGenerator
{
    public int GuidLength { get; set; }

    public UniqueGuidGenerator(int guidLength)
    {
        GuidLength = guidLength;
    }

    public string NewUniqueGuid(List<string> ids)
    {
        string newId = NewGuid();

        while (ids.Contains(newId))
        {
            newId = NewGuid();
        }
        return newId;
    }

    private string NewGuid()
    {
        return Guid.NewGuid().ToString()[GuidLength..];
    }
}
