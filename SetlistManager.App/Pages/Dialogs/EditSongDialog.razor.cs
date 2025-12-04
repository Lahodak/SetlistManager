using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class EditSongDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Parameter]
    public required SongModel Song { get; set; }
    [Inject]
    public required ISongService SongService { get; set; }
    [Inject]
    public required ILanguageService LanguageService { get; set; }
    [Inject]
    public required IArtistService ArtistService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private List<LanguageModel>? _languages;
    private List<ArtistModel>? _artists;

    protected override async Task OnInitializedAsync()
    {
        _languages = await LanguageService.GetAvailableLanguagesAsync();
        _artists = (await ArtistService.GetAvailableArtistsAsync(new() { PageSize = int.MaxValue }))?.Items;

        if (_artists is null || _artists.Count == 0)
        {
            Snackbar.Add("Add Artists First!", Severity.Warning);
        }
    }

    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Song.Name))
        {
            Snackbar.Add("Please provide Song Name", Severity.Error);
            return;
        }

        if (Song.Artist is null)
        {
            Snackbar.Add("Please select an Artist", Severity.Error);
            return;
        }

        if (Song.Language is null)
        {
            Snackbar.Add("Please select a Language", Severity.Error);
            return;
        }

        SongUpdateModel updateModel = new()
        {
            Name = Song.Name,
            ArtistId = Song.Artist.Id,
            LanguageId = Song.LanguageId,
            TabsURL = Song.TabsURL,
            AudioURL = Song.AudioURL,
            Tuning = Song.Tuning,
            Key = Song.Key,
            BPM = Song.BPM
        };

        if (await SongService.TryUpdateSongAsync(Song.Id, updateModel))
        {
            Snackbar.Add("Song updated successfully", Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Snackbar.Add("Failed to update Song", Severity.Error);
        }
    }

    private void Cancel() => MudDialog.Cancel();
}