using ScriptProfiler.Lib.Automation.Job;
using ScriptProfiler.Utility;
using MudBlazor;

namespace ScriptProfiler.Pages;

public sealed partial class ScriptJobResultTable
{
    private static readonly int[] s_pageSizeOptions = { 20, 50, 100 };

    private string _searchString = "";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            LoadSavedFiles();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void LoadSavedFiles()
    {
        if (ScriptJobService.Jobs.Count == 0)
        {
            ScriptJobSerializer.LoadFromSaveDirectory(ScriptJobService.Jobs);
        }

        if (ScriptJobService.JobResults.Count == 0)
        {
            ScriptJobResultSerializer.LoadFromSaveDirectory(ScriptJobService.JobResults);
        }
    }

    private void GoToJobResultDetailsPage(TableRowClickEventArgs<ScriptJobResult> eventArgs)
    {
        if (eventArgs.Item == null) return;

        int resultId = eventArgs.Item.Id;

        // TODO: toast notification of result not found error
        if (!ScriptJobService.JobResults.Contains(resultId)) return;

        NavigationManager.NavigateTo(PageRoute.ScriptJobResultDetailsWithId(resultId));
    }

    private bool FilterJobResultFunc(ScriptJobResult result) => DoFilterJobResultFunc(result, _searchString);

    private static bool DoFilterJobResultFunc(ScriptJobResult result, string searchString)
    {
        return string.IsNullOrWhiteSpace(searchString)
                    || result.Id.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    || result.ScriptName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    || result.ExecutionState.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase);
    }
}
