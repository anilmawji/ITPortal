using Microsoft.AspNetCore.Components;
using ScriptProfiler.Lib.Automation.Job;
using ScriptProfiler.Lib.Services;

namespace ScriptProfiler.Models;

public class ScriptJobResultBase : ComponentBase
{
    [Inject]
    public required IScriptJobService ScriptJobService { get; set; }

    [Parameter]
    public int Id { get; set; }

    public ScriptJobResult? Result { get; set; }

    protected override void OnInitialized()
    {
        Result = ScriptJobService.JobResults.GetResult(Id);
    }
}
