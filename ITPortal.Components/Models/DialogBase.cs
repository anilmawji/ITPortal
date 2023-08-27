using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ITPortal.Components.Models;

public class DialogBase : ComponentBase
{
    [CascadingParameter, EditorRequired]
    MudDialogInstance? MudDialog { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? ContentText { get; set; }

    [Parameter]
    public string SubmitButtonText { get; set; } = "Submit";

    [Parameter]
    public Color Color { get; set; }

    internal void Submit() => MudDialog?.Close(DialogResult.Ok(true));
    internal void Cancel() => MudDialog?.Cancel();
}
