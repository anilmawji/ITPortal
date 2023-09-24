using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Utility;
using System.Text.Json;

namespace ITPortal.Utility;

public static class ScriptJobResultFileHelper
{
    private static bool IsLoaded;
    private static readonly string JobResultsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "job_results");

    public static string GetJobResultJsonFilePath(int resultId)
    {
        return Path.Combine(JobResultsPath, resultId + ".json");
    }

    public static void LoadSavedJobResults(ScriptJobResultList resultList)
    {
        if (IsLoaded) return;
        IsLoaded = true;

        DirectoryInfo info = Directory.CreateDirectory(JobResultsPath);
        // Jobs folder has just been created; no jobs to load
        if (!info.Exists) return;

        IEnumerable<string> filePaths = Directory.EnumerateFiles(JobResultsPath);

        foreach (string path in filePaths)
        {
            ScriptJobResult result = LoadJobResultFromJsonFile(path);

            if (result == null) continue;

            TryAddJobResultToList(result, resultList);
        }
    }

    public static bool TryAddJobResultToList(ScriptJobResult result, ScriptJobResultList resultList)
    {
        if (resultList.HasResult(result.Id))
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to add job result to list: duplicate id \'{result.Id}\' found");

            return false;
        }
        resultList.Add(result);

        return true;
    }

    public static ScriptJobResult LoadJobResultFromJsonFile(string jsonFilePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(jsonFilePath);

        if (!int.TryParse(fileName, out int _))
        {
            Logger.AddMessage(LogEvent.Error, $"Failed to add job result to list: file name \'{fileName}\' is not an integer");

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

        return result;
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
