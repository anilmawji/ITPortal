using ITPortal.Lib.Automation.Script;

namespace ITPortal.Components.Shared;

public sealed partial class ScriptExecutionStateChip
{
    private static readonly IReadOnlyDictionary<ScriptExecutionState, MudBlazor.Color> MudColorMap = new Dictionary<ScriptExecutionState, MudBlazor.Color>()
    {
        { ScriptExecutionState.Running, MudBlazor.Color.Secondary },
        { ScriptExecutionState.Success, MudBlazor.Color.Success   },
        { ScriptExecutionState.Stopped, MudBlazor.Color.Warning   },
        { ScriptExecutionState.Error,   MudBlazor.Color.Error     }
    };

    private static readonly IReadOnlyDictionary<ScriptExecutionState, string> MudIconMap = new Dictionary<ScriptExecutionState, string>()
    {
        { ScriptExecutionState.Running, MudBlazor.Icons.Material.Outlined.AccessTime   },
        { ScriptExecutionState.Success, MudBlazor.Icons.Material.Outlined.CheckCircle  },
        { ScriptExecutionState.Stopped, MudBlazor.Icons.Material.Outlined.DoDisturb    },
        { ScriptExecutionState.Error,   MudBlazor.Icons.Material.Outlined.ErrorOutline }
    };

    protected override void OnInitialized()
    {
        Result.ExecutionResultReceived += OnExecutionResultReceived;
    }

    private void OnExecutionResultReceived(object? sender, ScriptExecutionState newState)
    {

        InvokeAsync(this.StateHasChanged);
    }

    private string GetIcon()
    {
        return MudIconMap[Result.ExecutionState];
    }

    private MudBlazor.Color GetColor()
    {
        return MudColorMap[Result.ExecutionState];
    }

    public void Dispose()
    {
        Result.ExecutionResultReceived -= OnExecutionResultReceived;
    }
}
