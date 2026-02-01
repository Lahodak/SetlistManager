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
        _artists = (await ArtistService.GetAvailableArtistsAsync(new() { PageSize = int.MaxValue, ContentType = ContentType.Private }))?.Items;

        if (_artists is null || _artists.Count == 0)
        {
            Snackbar.Add("Add Artists First!", Severity.Warning);
        }

        _song = new SongModel
        {
            Name = string.Empty,
            TabsURL = string.Empty,
            AudioURL = string.Empty,
            Tuning = string.Empty,
            Key = string.Empty,
            BPM = 120,
            IsPublic = false
        };
    }

    private Task<IEnumerable<ArtistModel>> SearchArtists(string value, CancellationToken token)
    {
        if (_artists is null)
            return Task.FromResult<IEnumerable<ArtistModel>>([]);

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
        if (_song is not null)
        {
            _song.Artist = selectedArtist;
        }
    }

    private void OnLanguageSelected(LanguageModel selectedLanguage)
    {
        if (_song is not null)
        {
            _song.Language = selectedLanguage;
        }
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
            BPM = _song.BPM,
            IsPublic = _song.IsPublic
        };

        await SongService.UploadSongAsync(songCreateModel);
        Snackbar.Add("Song added successfully!", Severity.Success);
        MudDialog.Close(DialogResult.Ok(_song));
    }

    private void Cancel() => MudDialog.Cancel();
}