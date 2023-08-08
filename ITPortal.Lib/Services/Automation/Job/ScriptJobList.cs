using ITPortal.Lib.Utils;

namespace ITPortal.Lib.Services.Automation.Job;

public class ScriptJobList
{
    public int JobIdLength { get; private set; }
    public Dictionary<string, ScriptJob> Jobs = new();

    private readonly UniqueGuidGenerator _guidGenerator;

    public ScriptJobList(int jobIdLength)
    {
        JobIdLength = jobIdLength;
        _guidGenerator = new UniqueGuidGenerator(JobIdLength);
    }

    public void AddNewJob()
    {
        string jobId = _guidGenerator.NewUniqueGuid(Jobs.Keys.ToList());

        Jobs.Add(jobId, new ScriptJob(jobId));
    }
}
