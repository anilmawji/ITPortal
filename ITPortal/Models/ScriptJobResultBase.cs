using ITPortal.Lib.Services.Automation.Job;
using Microsoft.AspNetCore.Components;

namespace ITPortal.Models;

public class ScriptJobResultBase : ComponentBase
{
    [Inject]
    public IScriptJobService ScriptJobService { get; set; }

    [Parameter]
    public int ResultId { get; set; }

    public ScriptJobResult Result { get; set; }

    protected override void OnInitialized()
    {
        Result = ScriptJobService.GetJobResult(ResultId);

        System.Diagnostics.Debug.WriteLine(Result.OutputStreamService.Output.Count);
    }
}
