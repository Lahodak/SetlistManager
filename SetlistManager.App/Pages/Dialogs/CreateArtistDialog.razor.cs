using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class CreateArtistDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Inject]
    public required IArtistService ArtistService { get; set; }

    private readonly ArtistCreateModel _artistCreateModel = new();

    private async Task Save()
    {
        if (string.IsNullOrEmpty(_artistCreateModel.Nick))
            return;

        await ArtistService.TryCreateArtistAsync(_artistCreateModel);
        MudDialog.Close(DialogResult.Ok(_artistCreateModel));
    }

    private void Cancel() => MudDialog.Cancel();
}