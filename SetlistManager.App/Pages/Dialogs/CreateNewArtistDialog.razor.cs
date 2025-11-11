using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class CreateNewArtistDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Inject]
    public required IArtistService ArtistService { get; set; }

    private readonly ArtistModel _artistModel = new();

    private async Task Save()
    {
        if (_artistModel.Nick == string.Empty)
            return;
        await ArtistService.UploadArtistAsync(_artistModel);
        MudDialog.Close(DialogResult.Ok(_artistModel));
    }

    private void Cancel() => MudDialog.Cancel();
}