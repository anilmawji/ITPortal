using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Job.Result;
using ITPortal.Lib.Utilities;
using Microsoft.Graph.Models;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace ITPortal.Utilities;

public class ScriptJobFileHelper
{
    private static readonly string JobsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "jobs");
    private static readonly string JobResultsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "job_results");

    private static bool IsJobsLoaded = false;
    private static bool IsJobResultsLoaded = false;

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
            Logger.AddMessage(LogEvent.Error, "Failed to add job to list: duplicate job name found");
            return null;
        }

        ScriptJob job;

        try
        {
            job = JsonSerializer.Deserialize(File.ReadAllText(jsonFilePath), ScriptJobContext.Default.ScriptJob);
        }
        catch (JsonException e)
        {
            Logger.AddMessage(LogEvent.Error, "Failed to deserialize job" + e.Message);
            return null;
        }

        if (fileName != job.Name)
        {
            Logger.AddMessage(LogEvent.Warning, "Warning - file name does not match job name: " + jsonFilePath);
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
            Logger.AddMessage(LogEvent.Error, "Failed to add job to list: file name is not an integer");
            return null;
        }

        if (resultList.HasResult(resultId))
        {
            Logger.AddMessage(LogEvent.Error, "Failed to add job to list: duplicate job name found");
            return null;
        }

        ScriptJobResult result;

        try
        {
            result = JsonSerializer.Deserialize(File.ReadAllText(jsonFilePath), ScriptJobResultContext.Default.ScriptJobResult);
        }
        catch (JsonException e)
        {
            Logger.AddMessage(LogEvent.Error, "Failed to deserialize job" + e.Message);
            return null;
        }

        if (fileName != result.Id.ToString())
        {
            Logger.AddMessage(LogEvent.Warning, "Warning - file name does not match job name: " + jsonFilePath);
        }

        resultList.Add(result);

        return result;
    }

    public static bool TryCreateJobFile(ScriptJob job)
    {
        try
        {
            string filePath = Path.Combine(JobsPath, job.Name + ".json");

            File.WriteAllText(filePath, job.ToJsonString());

            return true;
        }
        catch (Exception e)
        {
            Logger.AddMessage(LogEvent.Error, e.Message);

            return false;
        }
    }

    public static bool TryDeleteJobFile(string jobName)
    {
        try
        {
            string filePath = Path.Combine(JobsPath, jobName + ".json");

            File.Delete(filePath);

            return true;
        }
        catch (Exception e)
        {
            Logger.AddMessage(LogEvent.Error, e.Message);

            return false;
        }
    }

    public static bool TryCreateJobResultFile(ScriptJobResult result)
    {
        try
        {
            string filePath = Path.Combine(JobResultsPath, result.Id.ToString() + ".json");

            File.WriteAllText(filePath, result.ToJsonString());

            return true;
        }
        catch (Exception e)
        {
            Logger.AddMessage(LogEvent.Error, e.Message);

            return false;
        }
    }

    public static bool TryDeleteJobResultFile(int resultId)
    {
        try
        {
            string filePath = Path.Combine(JobResultsPath, resultId.ToString() + ".json");

            File.Delete(filePath);

            return true;
        }
        catch (Exception e)
        {
            Logger.AddMessage(LogEvent.Error, e.Message);

            return false;
        }
    }
}
