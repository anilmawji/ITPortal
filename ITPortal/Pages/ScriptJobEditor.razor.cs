﻿using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Script;
using ITPortal.Lib.Utility;
using ITPortal.Utility;

namespace ITPortal.Pages;

public sealed partial class ScriptJobEditor
{
    private static readonly PickOptions s_filePickOptions = new()
    {
        // PickerTitle is used by macOS but not Windows - unreliable for providing information to user
        PickerTitle = "Please select a PowerShell script file",
        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".ps1" } },
            { DevicePlatform.macOS, new[] { "ps1" } }
        })
    };

    private static string s_pageDescription;

    private MudBlazor.MudForm _jobForm;
    private string _headerTitle;
    private string[] _errors = Array.Empty<string>();

    private ScriptJob _job;
    private string _initialJobName;
    private string _newJobName;
    private bool _creatingNewJob;
    private bool _jobHasValidState;
    private bool _canTrySaveJob;

    protected override void OnInitialized()
    {
        _canTrySaveJob = false;

        if (JobName == null)
        {
            s_pageDescription = "Create new job";
            _newJobName = string.Empty;
            _initialJobName = ScriptJobService.Jobs.GetUniqueDefaultJobName();
            _job = new ScriptJob(_initialJobName, new PowerShellScript());
            _headerTitle = "New Job";
            _creatingNewJob = true;
        }
        else
        {
            s_pageDescription = "Modify job properties";
            _initialJobName = JobName;
            _newJobName = _initialJobName;
            _job = ScriptJobService.Jobs.GetJob(JobName);
            _headerTitle = $"Edit Job \"{JobName}\"";
        }
    }

    private void OnJobFieldChanged()
    {
        if (_job.Script.IsContentLoaded())
        {
            _canTrySaveJob = true;
        }
    }

    private IEnumerable<string> ValidateJobName(string jobName)
    {
        // If the current job name is blank, the default job name will be used upon form submission
        if (jobName.Length < 3 && jobName.Length > 0)
        {
            yield return "Job name must be at least 3 characters in length";
        }
        if (jobName.Length > 30)
        {
            yield return "Job name must be less than 30 characters in length";
        }
        if (ScriptJobService.Jobs.Contains(jobName) && (_creatingNewJob || jobName != _initialJobName))
        {
            yield return "Job name must be unique";
        }
        if (!jobName.IsValidFileName())
        {
            yield return "Job name must contain only letters and digits";
        }
    }

    private IEnumerable<string> ValidateJobDescription(string jobDescription)
    {
        if (jobDescription.Length > 100)
        {
            yield return "Job description must be less than 100 characters in length";
        }
    }

    private async Task PickAndLoadScript()
    {
        FileResult fileResult = await FileHelper.PickFileAsync(s_filePickOptions);

        if (fileResult == null) return;

        _job.Script.LoadContent(fileResult.FullPath);

        string[] errors = _job.Script.LoadParameters();

        if (errors.Length > 0)
        {
            Logger.AddMessages(LogEvent.Error, errors);
        }

        _canTrySaveJob = true;
        StateHasChanged();
    }

    private void RefreshScript()
    {
        string[] errors = _job.Script.Refresh();

        if (errors.Length > 0)
        {
            Logger.AddMessages(LogEvent.Error, errors);
        }
        else
        {
            _canTrySaveJob = true;
        }
        InvokeAsync(StateHasChanged);
    }

    private void EditScript()
    {
        if (!_job.Script.IsContentLoaded()) return;

        FileHelper.OpenFileWithDefaultProgram(_job.Script.FilePath);
    }

    private void RemoveScript()
    {
        _job.Script.Unload();

        _canTrySaveJob = false;
        InvokeAsync(StateHasChanged);
    }

    private void CancelJobChanges()
    {
        if (!_creatingNewJob && _canTrySaveJob)
        {
            string filePath = ScriptJobSerializer.GetFilePath(_job.Name);
            ScriptJob job = ScriptJobSerializer.LoadFromFile(filePath);

            ScriptJobService.Jobs.ReplaceJob(job);
        }
        NavigationManager.NavigateTo(PageRoute.ScriptJobs);
    }

    private async Task TrySaveJob()
    {
        await _jobForm.Validate();

        if (!_jobHasValidState) return;

        if (_creatingNewJob)
        {
            ScriptJobService.Jobs.Add(_job);
        }
        else
        {
            ScriptJobSerializer.TryDeleteFile(_job.Name);
        }

        if (_newJobName != string.Empty && _newJobName != _initialJobName)
        {
            ScriptJobService.Jobs.UpdateJobName(_job, _newJobName);
        }

        string filePath = ScriptJobSerializer.GetFilePath(_job.Name);
        ScriptJobSerializer.TryCreateFile(_job, filePath);

        NavigationManager.NavigateTo(PageRoute.ScriptJobs);
    }
}
