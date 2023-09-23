using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Job.Result;
using ITPortal.Lib.Utility;
using System.Text.Json;

namespace ITPortal.Utility;

public class ScriptJobFileHelper
{
    private static readonly string JobsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "jobs");
    private static readonly string JobResultsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "job_results");

    private static bool IsJobsLoaded = false;
    private static bool IsJobResultsLoaded = false;

    public static bool Loaded()
    {
        return IsJobsLoaded && IsJobResultsLoaded;
    }

    public static void LoadSavedJobs(ScriptJobList list)
    {
        if (IsJobsLoaded) return;
        IsJobsLoaded = true;

        DirectoryInfo info = Directory.CreateDirectory(JobsPath);
        // Jobs folder has just been created; no jobs to load
        if (!info.Exists) return;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(JobsPath);

        foreach (string path in filePaths)
        {
            LoadJobFromJsonFile(path, list);
        }
    }

    public static ScriptJob LoadJobFromJsonFile(string jsonFilePath, ScriptJobList jobList)
    {
        string fileName = Path.GetFileNameWithoutExtension(jsonFilePath);

        if (jobList.HasJob(fileName))
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to add job to list: duplicate job name \'{fileName}\' found");

            return null;
        }

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

        jobList.Add(job);

        return job;
    }

    public static void LoadSavedJobResults(ScriptJobResultList list)
    {
        if (IsJobResultsLoaded) return;
        IsJobResultsLoaded = true;

        DirectoryInfo info = Directory.CreateDirectory(JobResultsPath);
        // Jobs folder has just been created; no jobs to load
        if (!info.Exists) return;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(JobResultsPath);

        foreach (string path in filePaths)
        {
            LoadJobResultFromJsonFile(path, list);
        }
    }

    public static ScriptJobResult LoadJobResultFromJsonFile(string jsonFilePath, ScriptJobResultList resultList)
    {
        string fileName = Path.GetFileNameWithoutExtension(jsonFilePath);

        if (!int.TryParse(fileName, out int resultId))
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to add job result to list: file name \'{fileName}\' is not an integer");

            return null;
        }

        if (resultList.HasResult(resultId))
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to add job result to list: duplicate id \'{resultId}\' found");

            return null;
        }

        ScriptJobResult result;

        try
        {
            result = JsonSerializer.Deserialize(File.ReadAllText(jsonFilePath), ScriptJobResultContext.Default.ScriptJobResult);
        }
        catch (JsonException e)
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to deserialize job result \'{fileName}\': {e.Message}");

            return null;
        }

        if (fileName != result.Id.ToString())
        {
            Logger.AddMessage(LogEvent.Warning, $"Warning - file name of \'{jsonFilePath}\' does not match job result id \'{result.Id}\'");
        }

        if (Path.GetDirectoryName(jsonFilePath) != JobResultsPath)
        {
            try
            {
                File.Copy(jsonFilePath, Path.Combine(JobResultsPath, Path.GetFileName(jsonFilePath)));
            }
            catch (Exception e)
            {
                Logger.AddMessage(LogEvent.Warning, $"Failed to copy job result file to \'{JobResultsPath}\': {e.Message}");
            }
        }

        resultList.Add(result);

        return result;
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

    public static bool TryCreateJobResultFile(ScriptJobResult result, string jsonFilePath)
    {
        try
        {
            File.WriteAllText(jsonFilePath, result.ToJsonString());

            return true;
        }
        catch (Exception e)
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to create job result file \'{jsonFilePath}\': {e.Message}");

            return false;
        }
    }

    public static bool TryCreateJobResultFile(ScriptJobResult result)
    {
        return TryCreateJobResultFile(result, Path.Combine(JobResultsPath, result.Id.ToString() + ".json"));
    }

    public static bool TryDeleteJobResultFile(int resultId)
    {
        string jsonFilePath = Path.Combine(JobResultsPath, resultId.ToString() + ".json");

        try
        {
            File.Delete(jsonFilePath);

            return true;
        }
        catch (Exception e)
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to delete job result file \'{jsonFilePath}\': {e.Message}");

            return false;
        }
    }
}
