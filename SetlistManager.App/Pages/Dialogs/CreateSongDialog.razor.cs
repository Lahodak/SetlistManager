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
    private readonly SongCreateModel _songModel = new();

    private ArtistModel? _selectedArtist;
    private LanguageModel? _selectedLanguage;

    protected override async Task OnInitializedAsync()
    {
        _languages = await LanguageService.GetAvailableLanguagesAsync();
    }

    private async Task<IEnumerable<ArtistModel>> SearchArtists(string value, CancellationToken token)
    {
        var request = new ContentPagedRequest
        {
            PageSize = 10,
            Query = value
        };
        var result = await ArtistService.GetArtistsAsync(request);
        return result?.Items ?? [];
    }

    private Task<IEnumerable<LanguageModel>> SearchLanguages(string value, CancellationToken token)
    {
        if (_languages is null)
            return Task.FromResult<IEnumerable<LanguageModel>>([]);

        if (string.IsNullOrWhiteSpace(value))
            return Task.FromResult<IEnumerable<LanguageModel>>(_languages);

        var searchResults = _languages
            .Where(l => l.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult<IEnumerable<LanguageModel>>(searchResults);
    }

    private void OnArtistSelected(ArtistModel? selectedArtist)
    {
        _selectedArtist = selectedArtist;
        _songModel.ArtistId = selectedArtist?.Id;
    }

    private void OnLanguageSelected(LanguageModel? selectedLanguage)
    {
        _selectedLanguage = selectedLanguage;
        _songModel.LanguageId = selectedLanguage?.Id;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(_songModel.Name))
        {
            Snackbar.Add("Please provide Song Name", Severity.Warning);
            return;
        }

        if (_songModel.Name.Length < 2)
        {
            Snackbar.Add("Song name must be at least 2 characters", Severity.Warning);
            return;
        }

        if (_songModel.ArtistId is null)
        {
            Snackbar.Add("Please select an Artist", Severity.Warning);
            return;
        }

        if (_songModel.LanguageId is null)
        {
            Snackbar.Add("Please select a Language", Severity.Warning);
            return;
        }

        var result = await SongService.TryCreateSongAsync(_songModel);

        if (!result)
        {
            Snackbar.Add("Failed to create song", Severity.Error);
            return;
        }

        Snackbar.Add("Song created successfully!", Severity.Success);
        MudDialog.Close();
    }

    private void Cancel() => MudDialog.Cancel();
}