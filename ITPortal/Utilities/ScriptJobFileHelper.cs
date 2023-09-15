using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Job.Result;

namespace ITPortal.Utilities
{
    public class ScriptJobFileHelper
    {
        public static readonly string JobsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "jobs");
        public static readonly string JobResultsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "job_results");

        public static bool CreateJobFile(ScriptJob job)
        {
            return FileHelper.CreateFile(job.Name, "json", JobsPath, job.ToJsonString());
        }

        public static bool DeleteJobFile(string jobName)
        {
            return FileHelper.DeleteFile(jobName, "json", JobsPath);
        }

        public static bool CreateJobResultFile(ScriptJobResult result)
        {
            return FileHelper.CreateFile(result.Id.ToString(), "json", JobResultsPath, result.ToJsonString());
        }

        public static bool DeleteJobResultFile(int resultId)
        {
            return FileHelper.DeleteFile(resultId.ToString(), "json", JobResultsPath);
        }
    }
}
