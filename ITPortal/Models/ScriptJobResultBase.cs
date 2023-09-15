using ITPortal.Lib.Automation.Job.Result;
using ITPortal.Lib.Services;
using Microsoft.AspNetCore.Components;

namespace ITPortal.Models;

public class ScriptJobResultBase : ComponentBase
{
    [Inject]
    public IScriptJobService ScriptJobService { get; set; }

    [Parameter]
    public int Id { get; set; }

    public ScriptJobResult Result { get; set; }

    protected override void OnInitialized()
    {
        ScriptJobResult result = ScriptJobService.JobResultList.TryGetResult(Id);

        if (result != null)
        {
            Result = result;
        }
    }
}
