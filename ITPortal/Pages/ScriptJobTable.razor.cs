using CommunityToolkit.Maui.Storage;
using ITPortal.Components.Models.Dialog;
using ITPortal.Components.Shared.Dialog;
using ITPortal.Components.Utility;
using ITPortal.Lib.Automation.Job;
using ITPortal.Lib.Automation.Output;
using ITPortal.Utility;
using MudBlazor;
using Color = MudBlazor.Color;

namespace ITPortal.Pages;

public sealed partial class ScriptJobTable
{
    private static readonly DialogOptions s_dialogOptions = new()
    {
        CloseButton = true,
        DisableBackdropClick = true,
        MaxWidth = MaxWidth.ExtraSmall
    };

    private static readonly DialogParameters<Dialog> s_deleteJobDialogParameters = new();
    private static readonly DialogParameters<RunScriptJobDialog> s_runJobDialogParameters = new();
    private static readonly DialogParameters<Dialog> s_cancelJobDialogParameters = new();
    private static readonly int[] s_pageSizeOptions = { 20, 50, 100 };

    private static readonly PickOptions s_filePickOptions = new()
    {
        // PickerTitle is used by macOS but not Windows - unreliable for providing information to user
        PickerTitle = "Please select a Job file",
        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".json" } },
            { DevicePlatform.macOS, new[] { "json" } }
        })
    };

    private string _searchString = "";
    private HashSet<ScriptJob> selectedJobs = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            InitializeDialogParameters();
            ScriptJobFileHelper.LoadSavedJobs(ScriptJobService.JobList);
            ScriptJobResultFileHelper.LoadSavedJobResults(ScriptJobService.JobResultList);
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void InitializeDialogParameters()
    {
        s_deleteJobDialogParameters.Add(dialog => dialog.ContentText, "Are you sure you want to delete this job? This action cannot be undone.");
        s_deleteJobDialogParameters.Add(dialog => dialog.SubmitButtonText, "Delete");
        s_deleteJobDialogParameters.Add(dialog => dialog.Color, Color.Error);

        s_runJobDialogParameters.Add(dialog => dialog.SubmitButtonText, "Done");
        s_runJobDialogParameters.Add(dialog => dialog.Color, Color.Secondary);

        s_cancelJobDialogParameters.Add(dialog => dialog.ContentText, "Are you sure you want to stop execution of this job?");
        s_cancelJobDialogParameters.Add(dialog => dialog.SubmitButtonText, "Yes");
        s_cancelJobDialogParameters.Add(dialog => dialog.CancelButtonText, "No");
        s_cancelJobDialogParameters.Add(dialog => dialog.Color, Color.Secondary);
    }

    private void GoToNewJobPage()
    {
        NavigationManager.NavigateTo(PageRoute.CreateScriptJob);
    }

    private async Task DownloadSelectedJobsAsync()
    {
        FolderPickerResult folderResult = await FolderPicker.Default.PickAsync(CancellationToken.None);

        if (!folderResult.IsSuccessful) return;

        foreach (ScriptJob selectedJob in selectedJobs)
        {
            foreach (string jobName in ScriptJobService.JobList.GetJobs().Keys)
            {
                if (jobName == selectedJob.Name)
                {
                    string jsonFilePath = Path.Combine(folderResult.Folder.Path, jobName + ".json");

                    ScriptJobFileHelper.TryCreateJobFile(selectedJob, jsonFilePath);
                    break;
                }
            }
        }
    }

    private async Task PickJobFileAsync()
    {
        FileResult fileResult = await FileHelper.PickFileAsync(s_filePickOptions);

        if (fileResult == null) return;

        ScriptJob job = ScriptJobFileHelper.LoadJobFromJsonFile(fileResult.FullPath);

        if (job == null) return;

        if (ScriptJobFileHelper.TryAddJobToList(job, ScriptJobService.JobList))
        {
            StateHasChanged();
        }
    }

    private async Task OpenRunJobDialog(ScriptJob job)
    {
        IDialogReference dialog = DialogService.Show<RunScriptJobDialog>("Run Job", s_runJobDialogParameters, s_dialogOptions);
        DialogResult dialogResult = await dialog.Result;

        if (dialogResult.Canceled) return;

        var dialogResultData = dialogResult.Data as RunScriptJobDialogResult;

        ScriptJobResult jobResult = RunJob(
            job, job.Script.NewScriptOutputList(),
            dialogResultData.DeviceName,
            dialogResultData.ShouldRunJobNow,
            dialogResultData.ErrorAction,
            dialogResultData.ShouldViewJobResult
        );

        if (dialogResultData.ShouldViewJobResult)
        {
            NavigationManager.NavigateTo(PageRoute.ScriptJobResultDetailsWithId(jobResult.Id));
        }
        else
        {
            StateHasChanged();
        }
    }

    private ScriptJobResult RunJob(ScriptJob job, ScriptOutputList outputList, string deviceName,
        bool runJobNow, ScriptJobErrorAction errorAction, bool refreshPage)
    {
        ScriptJobResult jobResult;

        if (errorAction == ScriptJobErrorAction.Cancel)
        {
            CancelJobOnErrorOutputReceived(job, outputList);
        }

        if (runJobNow)
        {
            jobResult = ScriptJobService.RunJob(job, deviceName, outputList);
        }
        else
        {
            //TODO: Schedule job to run at _runDate instead
            jobResult = ScriptJobService.RunJob(job, deviceName, outputList);
        }

        // This is executed when the job is finished running
        jobResult.RunJobTask.ContinueWith((resultTask) =>
        {
            ScriptJobResultFileHelper.TryCreateJobResultFile(jobResult);

            if (refreshPage)
            {
                InvokeAsync(StateHasChanged);
            }
            outputList.Dispose();
        });

        return jobResult;
    }

    private void CancelJobOnErrorOutputReceived(ScriptJob job, ScriptOutputList outputList)
    {
        outputList.OutputChanged += (object sender, ScriptOutputChangedEventArgs e) =>
        {
            if (e.StreamType == ScriptOutputStreamType.Error)
            {
                CancelJob(job);
            }
        };
    }

    private async Task OpenCancelJobDialog(ScriptJob job)
    {
        IDialogReference dialog = DialogService.Show<Dialog>("Confirm Job Cancellation", s_cancelJobDialogParameters, s_dialogOptions);
        DialogResult dialogResult = await dialog.Result;

        if (dialogResult.Canceled) return;

        CancelJob(job);
    }

    private void GoToEditJobPage(ScriptJob job)
    {
        // TODO: toast notification of job not found error
        if (!ScriptJobService.JobList.HasJob(job.Name)) return;

        NavigationManager.NavigateTo(PageRoute.EditScriptJobWithName(job.Name));
    }

    private async Task OpenDeleteJobDialog(string jobName)
    {
        IDialogReference dialog = DialogService.Show<DeleteScriptJobDialog>("Confirm Job Deletion", s_deleteJobDialogParameters, s_dialogOptions);
        DialogResult dialogResult = await dialog.Result;

        if (dialogResult.Canceled) return;

        var dialogResultData = dialogResult.Data as DeleteScriptJobDialogResult;
        ScriptJob job = ScriptJobService.JobList.TryGetJob(jobName);

        if (dialogResultData.ShouldDeleteJobResults && job != null)
        {
            DeleteJobResults(job);
        }
        DeleteJob(jobName);
    }

    private void CancelJob(ScriptJob job)
    {
        job.Cancel();

        InvokeAsync(StateHasChanged);
    }

    private void DeleteJob(string jobName)
    {
        ScriptJobService.JobList.Remove(jobName);
        ScriptJobFileHelper.TryDeleteJobFile(jobName);

        InvokeAsync(StateHasChanged);
    }

    private void DeleteJobResults(ScriptJob job)
    {
        List<ScriptJobResult> results = ScriptJobService.JobResultList.Remove(job);

        foreach (ScriptJobResult result in results)
        {
            ScriptJobResultFileHelper.TryDeleteJobResultFile(result.Id);
        }
    }

    private bool FilterJobFunc(ScriptJob job) => DoFilterJobFunc(job, _searchString);

    private static bool DoFilterJobFunc(ScriptJob job, string searchString)
    {
        return string.IsNullOrWhiteSpace(searchString)
                    || job.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    || job.Script.FileName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    || job.State.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    || job.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    }
}
