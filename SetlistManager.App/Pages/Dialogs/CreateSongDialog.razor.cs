using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class CreateSongDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }
    [Inject]
    public required ISongService SongService { get; set; }
    [Inject]
    public required ILanguageService LanguageService { get; set; }
    [Inject]
    public required IArtistService ArtistService { get; set; }

    private List<LanguageModel>? _languages;
    private List<ArtistModel>? _artists;
    private SongModel? _song;

    protected override async Task OnInitializedAsync()
    {
        _languages = await LanguageService.GetAvailableLanguagesAsync();
        _artists = await ArtistService.GetAvailableArtistsAsync();

        if (_artists is null || _artists.Count == 0)
        {
            Snackbar.Add("Add Artists First!", Severity.Warning);
        }

        _song = new SongModel
        {
            Name = string.Empty,
            Artist = null!,
            Language = null!,
            TabsURL = string.Empty,
            AudioURL = string.Empty,
            Tuning = string.Empty,
            Key = string.Empty,
            BPM = 120
        };
    }

    private async Task SaveSong()
    {
        if (_song is null)
            return;

        if (_artists is null || _artists.Count == 0)
        {
            Snackbar.Add("Add Artists First!", Severity.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(_song.Name))
        {
            Snackbar.Add("Please provide Song Name", Severity.Error);
            return;
        }

        if (_song.Artist is null)
        {
            Snackbar.Add("Please select an Artist", Severity.Error);
            return;
        }

        if (_song.Language is null)
        {
            Snackbar.Add("Please select a Language", Severity.Error);
            return;
        }

        _song.LanguageId = _song.Language.Id;

        SongCreateModel songCreateModel = new()
        {
            Name = _song.Name,
            ArtistId = _song.Artist.Id,
            LanguageId = _song.Language.Id,
            TabsURL = _song.TabsURL,
            AudioURL = _song.AudioURL,
            Key = _song.Key,
            Tuning = _song.Tuning,
            BPM = _song.BPM
        };
        await SongService.UploadSongAsync(songCreateModel);

        Snackbar.Add("Song added successfully!", Severity.Success);
        MudDialog.Close(DialogResult.Ok(_song));
    }

    private void Cancel() => MudDialog.Cancel();
}