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

    private Task<IEnumerable<ArtistModel>> SearchArtists(string value, CancellationToken token)
    {
        if (_artists is null)
            return Task.FromResult<IEnumerable<ArtistModel>>(new List<ArtistModel>());

        if (string.IsNullOrWhiteSpace(value))
            return Task.FromResult<IEnumerable<ArtistModel>>(_artists);

        var searchResults = _artists
            .Where(a => a.Nick.Contains(value, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult<IEnumerable<ArtistModel>>(searchResults);
    }

    private Task<IEnumerable<LanguageModel>> SearchLanguages(string value, CancellationToken token)
    {
        if (_languages is null)
            return Task.FromResult<IEnumerable<LanguageModel>>(new List<LanguageModel>());

        if (string.IsNullOrWhiteSpace(value))
            return Task.FromResult<IEnumerable<LanguageModel>>(_languages);

        var searchResults = _languages
            .Where(l => l.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult<IEnumerable<LanguageModel>>(searchResults);
    }

    private void OnArtistSelected(ArtistModel selectedArtist)
    {
        Song.Artist = selectedArtist;
    }

    private void OnLanguageSelected(LanguageModel selectedLanguage)
    {
        Song.Language = selectedLanguage;
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
            LanguageId = Song.Language.Id,
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