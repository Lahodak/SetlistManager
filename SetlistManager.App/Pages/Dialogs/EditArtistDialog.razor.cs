using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class EditArtistDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Parameter]
    public required ArtistModel ArtistToEdit { get; set; }
    [Inject]
    public required IArtistService ArtistService { get; set; }

    private ArtistUpdateModel? _artistUpdateModel;

    protected override void OnInitialized()
    {
        _artistUpdateModel = new ArtistUpdateModel
        {
            Nick = ArtistToEdit.Nick
        };
    }

    public async Task SaveAsync()
    {
        if (_artistUpdateModel is null)
            return;
        await ArtistService.TryUpdateArtistAsync(ArtistToEdit.Id, _artistUpdateModel);
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Cancel() => MudDialog.Cancel();
}