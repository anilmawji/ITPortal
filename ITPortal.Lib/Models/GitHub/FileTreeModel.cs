namespace ITPortal.Lib.Models.GitHub;

public sealed class FileTreeModel
{
    public string sha { get; set; }
    public string url { get; set; }
    public Tree[] tree { get; set; }
    public bool truncated { get; set; }
}

public sealed class Tree
{
    public string path { get; set; }
    public string mode { get; set; }
    public string type { get; set; }
    public string sha { get; set; }
    public int size { get; set; }
    public string url { get; set; }
}
