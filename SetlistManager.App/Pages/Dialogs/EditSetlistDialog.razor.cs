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
        var result = await SongService.GetSongsAsync(new() { PageSize = 10, ContentType = ContentType.Private });

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

    private async Task<IEnumerable<SongModel>> Search(string value, CancellationToken token)
    {
        var request = new PagedRequest
        {
            PageSize = 5,
            Query = value,
            ContentType = ContentType.Private
        };

        var result = await SongService.GetSongsAsync(request);

        if (result?.Items is null)
            return [];

        var availableSongs = result.Items
            .Where(s => !_setlist.Songs.Any(ss => ss.Id == s.Id))
            .ToList();

        return availableSongs;
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

        var result = await SetlistService.TryEditSetlist(_setlist);

        if (!result)
        {
            Snackbar.Add("Failed to save setlist", Severity.Error);
            return;
        }

        MudDialog.Close();
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

    private void MoveSongUp(SongModel song) => MoveSong(song, -1);

    private void MoveSongDown(SongModel song) => MoveSong(song, 1);

    private void MoveSong(SongModel song, int direction)
    {
        int index = _setlist.Songs.IndexOf(song);
        int newIndex = index + direction;

        if (newIndex < 0 || newIndex >= _setlist.Songs.Count)
            return;

        (_setlist.Songs[newIndex], _setlist.Songs[index]) =
            (_setlist.Songs[index], _setlist.Songs[newIndex]);

        ReorderSongs();
        StateHasChanged();
    }

    private void ReorderSongs()
    {
        for (int i = 0; i < _setlist.Songs.Count; i++)
        {
            _setlist.Songs[i].Order = i + 1;
        }
    }
}