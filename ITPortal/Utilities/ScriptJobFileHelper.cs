using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Job.Result;
using ITPortal.Lib.Utilities;

namespace ITPortal.Utilities
{
    public class ScriptJobFileHelper
    {
        public static readonly string JobsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "jobs");
        public static readonly string JobResultsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "job_results");

        public static bool CreateJobFile(ScriptJob job)
        {
            try
            {
                string filePath = Path.Combine(JobsPath, job.Name + ".json");

                File.WriteAllText(filePath, job.ToJsonString());

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToFile(LogEvent.Error, e.Message);

                return false;
            }
        }

        public static bool DeleteJobFile(string jobName)
        {
            try
            {
                string filePath = Path.Combine(JobsPath, jobName + ".json");

                File.Delete(filePath);

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToFile(LogEvent.Error, e.Message);

                return false;
            }
        }

        public static bool CreateJobResultFile(ScriptJobResult result)
        {
            try
            {
                string filePath = Path.Combine(JobResultsPath, result.Id.ToString() + ".json");

                File.WriteAllText(filePath, result.ToJsonString());

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToFile(LogEvent.Error, e.Message);

                return false;
            }
        }

        public static bool DeleteJobResultFile(int resultId)
        {
            try
            {
                string filePath = Path.Combine(JobResultsPath, resultId.ToString() + ".json");

                File.Delete(filePath);

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteToFile(LogEvent.Error, e.Message);

                return false;
            }
        }
    }
}
