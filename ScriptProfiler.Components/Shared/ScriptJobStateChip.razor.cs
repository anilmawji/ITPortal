using ScriptProfiler.Lib.Automation.Job;

namespace ScriptProfiler.Components.Shared;

public sealed partial class ScriptJobStateChip
{
    private static readonly IReadOnlyDictionary<ScriptJobState, string> MudIconMap = new Dictionary<ScriptJobState, string>()
    {
        { ScriptJobState.Scheduled, MudBlazor.Icons.Material.Outlined.CalendarMonth },
        { ScriptJobState.Running,   MudBlazor.Icons.Material.Outlined.AccessTime    }
    };

    protected override void OnInitialized()
    {
        Job.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(object? sender, ScriptJobState newState) => InvokeAsync(StateHasChanged);

    private string GetIcon()
    {
        return MudIconMap[Job.State];
    }

    public void Dispose()
    {
        Job.StateChanged -= OnStateChanged;
    }
}
