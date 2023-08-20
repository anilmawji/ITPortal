using ITPortal.Lib.Services.Automation.Job;
using Microsoft.AspNetCore.Components;

namespace ITPortal.Models;

public class ScriptJobDetailsBase : ComponentBase
{
    [Inject]
    public IScriptJobService ScriptJobService { get; set; }

    [Parameter]
    public int? Id { get; set; }

    public ScriptJob Job { get; set; }

    protected override void OnInitialized()
    {
        Id ??= 0;
        Job = ScriptJobService.GetJobOrDefault((int)Id);
    }
}
