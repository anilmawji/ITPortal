using ITPortal.Lib.Automation.Job;
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
        Result = ScriptJobService.GetJobResult(Id);
    }
}
