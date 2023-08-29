﻿namespace ITPortal.Components.Utilities;

public class PageRoute
{
    public const string Login = "/login";

    public const string CreateScriptJob = "/create-job";
    public const string EditScriptJob = "/create-job/{JobName}";
    public const string ScriptJobs = "/jobs";
    public const string ScriptJobResults = "/job-results";
    public const string ScriptJobResultDetails = "/job-results/{Id:int}";

    public const string DeviceInventory = "/devices";

    public static string ScriptJobResultDetailsWithId(int jobId)
    {
        return $"{ScriptJobResults}/{jobId}";
    }

    public static string EditScriptJobWithName(string jobName)
    {
        return $"{CreateScriptJob}/{jobName}";
    }

    // Only applicable to routes without route parameters
    public static string GetPageTitleFromRoute(string url)
    {
        return url switch
        {
            ScriptJobs => "PowerShell Jobs",
            ScriptJobResults => "PowerShell Job Results",
            CreateScriptJob => "Create Script Job",
            DeviceInventory => "Device Inventory",
            _ => throw new ArgumentOutOfRangeException(nameof(url)),
        };
    }
}
