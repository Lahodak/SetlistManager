using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class EditSetlistDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public SetlistModel Setlist { get; set; } = new();

    [Inject]
    public required ISetlistService SetlistService { get; set; }

    [Inject]
    public required ISongService SongService { get; set; }

    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private SetlistModel _setlist = new();
    private List<SongModel>? _allSongs = [];

    protected override async Task OnInitializedAsync()
    {
        var result = await SongService.GetAllSongsAsync(new() { PageSize = int.MaxValue });

        _allSongs = result?.Items;

        _setlist = new SetlistModel
        {
            Id = Setlist.Id,
            Name = Setlist.Name,
            OwnerId = Setlist.OwnerId,
            Songs = Setlist.Songs
        };

        for (int i = 0; i < _setlist.Songs.Count; i++)
        {
            _setlist.Songs[i].Order = i + 1;
        }
    }

    private Task<IEnumerable<SongModel>> Search(string value, CancellationToken token)
    {
        if (_allSongs is null)
            return Task.FromResult<IEnumerable<SongModel>>(new List<SongModel>());

        if (string.IsNullOrWhiteSpace(value))
        {
            var availableSongs = _allSongs
                .Where(s => !_setlist.Songs.Any(ss => ss.Id == s.Id))
                .ToList();
            return Task.FromResult<IEnumerable<SongModel>>(availableSongs);
        }

        var searchResults = _allSongs
            .Where(s => (s.Name?.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false) ||
                       (s.Artist?.Nick.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false))
            .Where(s => !_setlist.Songs.Any(ss => ss.Id == s.Id))
            .ToList();

        return Task.FromResult<IEnumerable<SongModel>>(searchResults);
    }

    private void OnSongSelected(SongModel selectedSong)
    {
        if (selectedSong == null) return;

        if (_setlist.Songs.Any(s => s.Id == selectedSong.Id))
        {
            Snackbar.Add("Song already in setlist", Severity.Info);
            return;
        }

        selectedSong.Order = _setlist.Songs.Count + 1;
        _setlist.Songs.Add(selectedSong);
        StateHasChanged();
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(_setlist.Name))
        {
            Snackbar.Add("Please enter a setlist name", Severity.Warning);
            return;
        }

        if (_setlist.Name.Length < 4)
        {
            Snackbar.Add("Setlist name has to be 4 characters or longer", Severity.Warning);
            return;
        }

        for (int i = 0; i < _setlist.Songs.Count; i++)
        {
            _setlist.Songs[i].Order = i + 1;
        }

        await SetlistService.EditSetlist(_setlist);
        MudDialog.Close(DialogResult.Ok(_setlist));
    }

    private void Cancel() => MudDialog.Cancel();

    private void RemoveSong(SongModel song)
    {
        _setlist.Songs.Remove(song);

        for (int i = 0; i < _setlist.Songs.Count; i++)
        {
            _setlist.Songs[i].Order = i + 1;
        }

        StateHasChanged();
    }

    private void MoveSongUp(SongModel song)
    {
        int index = _setlist.Songs.IndexOf(song);
        if (index <= 0) return;

        (_setlist.Songs[index - 1], _setlist.Songs[index]) = (_setlist.Songs[index], _setlist.Songs[index - 1]);

        for (int i = 0; i < _setlist.Songs.Count; i++)
        {
            _setlist.Songs[i].Order = i + 1;
        }

        StateHasChanged();
    }

    private void MoveSongDown(SongModel song)
    {
        int index = _setlist.Songs.IndexOf(song);
        if (index >= _setlist.Songs.Count - 1) return;

        (_setlist.Songs[index + 1], _setlist.Songs[index]) = (_setlist.Songs[index], _setlist.Songs[index + 1]);

        for (int i = 0; i < _setlist.Songs.Count; i++)
        {
            _setlist.Songs[i].Order = i + 1;
        }

        StateHasChanged();
    }
}