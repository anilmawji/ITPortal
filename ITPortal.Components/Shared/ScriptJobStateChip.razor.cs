using ITPortal.Lib.Automation.Job;

namespace ITPortal.Components.Shared;

public sealed partial class ScriptJobStateChip
{
    private static readonly IReadOnlyDictionary<ScriptJobState, MudBlazor.Color> MudColorMap = new Dictionary<ScriptJobState, MudBlazor.Color>()
    {
        { ScriptJobState.Idle,      MudBlazor.Color.Dark      },
        { ScriptJobState.Scheduled, MudBlazor.Color.Dark      },
        { ScriptJobState.Running,   MudBlazor.Color.Secondary }
    };

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

    private MudBlazor.Color GetColor()
    {
        return MudColorMap[Job.State];
    }

    public void Dispose()
    {
        Job.StateChanged -= OnStateChanged;
    }
}
