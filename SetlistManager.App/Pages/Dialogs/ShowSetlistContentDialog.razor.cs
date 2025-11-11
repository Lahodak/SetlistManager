using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class ShowSetlistContentDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public required SetlistModel Setlist { get; set; }

    [Parameter]
    public required int CurrentSongId { get; set; }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}