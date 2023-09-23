using Microsoft.AspNetCore.Components;

namespace ITPortal.Components.Models.Dialog;

public class DialogBase : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public MudBlazor.MudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? ContentText { get; set; }

    [Parameter]
    public string SubmitButtonText { get; set; } = "Submit";

    [Parameter]
    public string CancelButtonText { get; set; } = "Cancel";

    [Parameter]
    public MudBlazor.Color Color { get; set; }

    internal virtual void Submit() => MudDialog?.Close(MudBlazor.DialogResult.Ok(true));
    internal virtual void Cancel() => MudDialog?.Cancel();
}
