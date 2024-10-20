using ScriptProfiler.Lib.Automation.Job;
using ScriptProfiler.Lib.Automation.Output;
using ScriptProfiler.Utility;
using MudBlazor;

namespace ScriptProfiler.Pages;

public sealed partial class ScriptJobResultDetails
{
    private static readonly Dictionary<ScriptOutputStreamType, string> TabIcons = new()
    {
        { ScriptOutputStreamType.Standard,    Icons.Material.Rounded.Output              },
        { ScriptOutputStreamType.Information, Icons.Material.Rounded.Info                },
        { ScriptOutputStreamType.Progress,    Icons.Material.Rounded.AlignHorizontalLeft },
        { ScriptOutputStreamType.Warning,     Icons.Material.Rounded.Warning             },
        { ScriptOutputStreamType.Error,       Icons.Material.Rounded.Error               },
    };

    ScriptJob? Job;

    protected override void OnInitialized()
    {
        // Very important to make sure ScriptJobResultBase is populated
        base.OnInitialized();

        if (Result != null)
        {
            Job = ScriptJobService.Jobs.GetJob(Result.JobName);
        }
    }

    private static string GetTabName(ScriptOutputStreamType streamType)
    {
        if (streamType == ScriptOutputStreamType.Warning || streamType == ScriptOutputStreamType.Error)
        {
            return streamType.ToStringFast() + "S";
        }
        return streamType.ToStringFast();
    }

    private static string GetOutputLineCountText(IReadOnlyList<ScriptOutputMessage> messages)
    {
        int numMessages = messages.Count;

        return numMessages <= 99 ? numMessages.ToString() : "99+";
    }

    private void CancelJob()
    {
        Job?.Cancel();
    }

    private void GoToEditJob()
    {
        if (Job == null) return;

        NavigationManager.NavigateTo(PageRoute.EditScriptJobWithName(Job.Name));
    }
}
