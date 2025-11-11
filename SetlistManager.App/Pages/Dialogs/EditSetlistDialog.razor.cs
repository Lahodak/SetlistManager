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
    public required IDialogService dialogService { get; set; }

    private SetlistModel _setlist = new();

    protected override void OnInitialized()
    {
        _setlist = new SetlistModel
        {
            Id = Setlist.Id,
            Name = Setlist.Name,
            Songs = new List<SongModel>(Setlist.Songs)
        };
    }

    private async Task Save()
    {
        if (Setlist is null)
            return;

        await SetlistService.EditSetlist(_setlist);
        MudDialog.Close(DialogResult.Ok(_setlist));
    }

    private void Cancel() => MudDialog.Cancel();

    private void RemoveSong(SongModel song) => _setlist.Songs.Remove(song);

    private void MoveSongUp(SongModel song)
    {
        var songUp = _setlist.Songs.FirstOrDefault(s => s.Order == song.Order - 1) ?? song;

        if (songUp.Order == song.Order)
            return;

        song.Order = songUp.Order;
        songUp.Order = song.Order + 1;

        int index = _setlist.Songs.IndexOf(song);
        if (index > 0)
            (_setlist.Songs[index - 1], _setlist.Songs[index]) = (_setlist.Songs[index], _setlist.Songs[index - 1]);
    }

    private void MoveSongDown(SongModel song)
    {
        var songDown = _setlist.Songs.FirstOrDefault(s => s.Order == song.Order + 1) ?? song;

        if (songDown.Order == song.Order)
            return;

        song.Order = songDown.Order;
        songDown.Order = song.Order - 1;

        int index = _setlist.Songs.IndexOf(song);
        if (index < _setlist.Songs.Count - 1)
            (_setlist.Songs[index + 1], _setlist.Songs[index]) = (_setlist.Songs[index], _setlist.Songs[index + 1]);
    }
}