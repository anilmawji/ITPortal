using ITPortal.Lib.Automation.Job;
using ITPortal.Utility;
using MudBlazor;

namespace ITPortal.Pages;

public sealed partial class ScriptJobResultTable
{
    private static readonly int[] s_pageSizeOptions = { 20, 50, 100 };

    private string _searchString = "";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ScriptJobFileHelper.LoadSavedJobs(ScriptJobService.JobList);
            ScriptJobFileHelper.LoadSavedJobResults(ScriptJobService.JobResultList);
            this.StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void TryGoToJobResultDetailsPage(TableRowClickEventArgs<ScriptJobResult> eventArgs)
    {
        int resultId = eventArgs.Item.Id;

        // TODO: toast notification of result not found error
        if (!ScriptJobService.JobResultList.HasResult(resultId)) return;

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
