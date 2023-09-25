using System.Collections;

namespace ITPortal.Lib.Automation.Job;

public class ScriptJobCollection : ICollection<ScriptJob>
{
    public int Count => Jobs.Count;
    public bool IsReadOnly => false;

    private readonly Dictionary<string, ScriptJob> Jobs = new();

    public void Add(ScriptJob job)
    {
        Jobs.Add(job.Name, job);
    }

    public bool TryAdd(ScriptJob job)
    {
        if (!Contains(job.Name))
        {
            Jobs.Add(job.Name, job);

            return true;
        }
        return false;
    }

    public bool Remove(ScriptJob job) => Remove(job.Name);

    public bool Remove(string jobName)
    {
        return Jobs.Remove(jobName);
    }

    public bool ReplaceJob(ScriptJob newJob)
    {
        if (Remove(newJob.Name))
        {
            Add(newJob);

            return true;
        }
        return false;
    }

    public string GetUniqueDefaultJobName()
    {
        string name = $"Job({Jobs.Count})";

        while (Contains(name))
        {
            name += "(1)";
        }
        return name;
    }

    public bool UpdateJobName(ScriptJob job, string newJobName)
    {
        if (!Remove(job.Name))
        {
            return false;
        }
        job.Name = newJobName;
        Add(job);

        return true;
    }

    public ScriptJob? GetJob(string jobName)
    {
        Jobs.TryGetValue(jobName, out ScriptJob? result);

        return result;
    }

    public void Clear() => Jobs.Clear();

    public bool Contains(ScriptJob job) => Jobs.ContainsKey(job.Name);

    public bool Contains(string jobName) => Jobs.ContainsKey(jobName);

    public void CopyTo(ScriptJob[] array, int arrayIndex)
    {
        int index = arrayIndex;

        foreach (ScriptJob job in Jobs.Values)
        {
            array[index++] = job;
        }
    }

    public IEnumerator<ScriptJob> GetEnumerator() => Jobs.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Jobs.Values.GetEnumerator();
}
