namespace ITPortal.Lib.Models.GitHub;

public sealed class BranchModel
{
    public string name { get; set; }
    public Commit commit { get; set; }
    public _Links _links { get; set; }
    public bool _protected { get; set; }
    public string protection_url { get; set; }
}

public sealed class Commit
{
    public string sha { get; set; }
    public string node_id { get; set; }
    public Commit1 commit { get; set; }
    public string url { get; set; }
    public string html_url { get; set; }
    public string comments_url { get; set; }
}

public sealed class Commit1
{
    public Author author { get; set; }
    public Committer committer { get; set; }
    public string message { get; set; }
    public Tree tree { get; set; }
    public string url { get; set; }
    public int comment_count { get; set; }
}

public sealed class Author
{
    public string name { get; set; }
    public string email { get; set; }
    public DateTime date { get; set; }
}

public sealed class Committer
{
    public string name { get; set; }
    public string email { get; set; }
    public DateTime date { get; set; }
}
