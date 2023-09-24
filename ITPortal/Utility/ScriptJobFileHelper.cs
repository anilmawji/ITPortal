using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Utility;
using System.Text.Json;

namespace ITPortal.Utility;

public static class ScriptJobFileHelper
{
    private static bool IsLoaded;
    private static readonly string JobsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "jobs");

    public static string GetJobJsonFilePath(string jobName)
    {
        return Path.Combine(JobsPath, jobName + ".json");
    }

    public static bool TryReloadJobFromJsonFile(string jsonFilePath, ScriptJobList jobList)
    {
        ScriptJob job = LoadJobFromJsonFile(jsonFilePath);

        if (jobList.Remove(job.Name))
        {
            jobList.Add(job);

            return true;
        }
        return false;
    }

    public static void LoadSavedJobs(ScriptJobList jobList)
    {
        if (IsLoaded) return;
        IsLoaded = true;

        DirectoryInfo info = Directory.CreateDirectory(JobsPath);
        // Jobs folder has just been created; no jobs to load
        if (!info.Exists) return;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(JobsPath);

        foreach (string path in filePaths)
        {
            ScriptJob job = LoadJobFromJsonFile(path);

            if (job == null) continue;

            TryAddJobToList(job, jobList);
        }
    }

    public static bool TryAddJobToList(ScriptJob job, ScriptJobList jobList)
    {
        if (jobList.HasJob(job.Script.FileName))
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to add job to list: duplicate job name \'{job.Script.FileName}\' found");

            return false;
        }
        jobList.Add(job);

        return true;
    }

    public static ScriptJob LoadJobFromJsonFile(string jsonFilePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(jsonFilePath);
        ScriptJob job;

        try
        {
            job = JsonSerializer.Deserialize(File.ReadAllText(jsonFilePath), ScriptJobContext.Default.ScriptJob);
        }
        catch (JsonException e)
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to deserialize job \'{fileName}\': {e.Message}");

            return null;
        }

        if (fileName != job.Name)
        {
            Logger.AddMessage(LogEvent.Warning, $"Warning - file name of \'{jsonFilePath}\' does not match job name \'{job.Name}\'");
        }

        if (Path.GetDirectoryName(jsonFilePath) != JobsPath)
        {
            try
            {
                File.Copy(jsonFilePath, Path.Combine(JobsPath, Path.GetFileName(jsonFilePath)));
            }
            catch (Exception e)
            {
                Logger.AddMessage(LogEvent.Warning, $"Failed to copy job file to \'{JobsPath}\': {e.Message}");
            }
        }

        if (job.Script.FilePath != null)
        {
            job.Script.LoadContent(job.Script.FilePath);
        }

        return job;
    }

    public static bool TryCreateJobFile(ScriptJob job, string jsonFilePath)
    {
        try
        {
            File.WriteAllText(jsonFilePath, job.ToJsonString());

            return true;
        }
        catch (Exception e)
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to create job file \'{jsonFilePath}\': {e.Message}");

            return false;
        }
    }

    public static bool TryCreateJobFile(ScriptJob job)
    {
        return TryCreateJobFile(job, Path.Combine(JobsPath, job.Name + ".json"));
    }

    public static bool TryDeleteJobFile(string jobName)
    {
        string jsonFilePath = Path.Combine(JobsPath, jobName + ".json");

        try
        {
            File.Delete(jsonFilePath);

            return true;
        }
        catch (Exception e)
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to delete job file \'{jsonFilePath}\': {e.Message}");

            return false;
        }
    }
}
