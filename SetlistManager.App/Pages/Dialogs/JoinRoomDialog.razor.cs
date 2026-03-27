using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SetlistManager.App.Pages.Dialogs;

public partial class JoinRoomDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    private string? _roomCode;

    private void JoinRoom()
    {
        if (!string.IsNullOrWhiteSpace(_roomCode))
        {
            NavigationManager.NavigateTo($"/room/{_roomCode}");
            MudDialog.Close();
        }
    }

    private void Cancel() => MudDialog.Cancel();
}