using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class SongDetail
{
	[Parameter]
	public int SongId { get; set; }
    [Inject]
	public required SongsDB SongsDatabase { get; set; }   	

    SongModel song = new();

	protected override async Task OnInitializedAsync()
	{
		if (SongsDatabase.GetCount() == 0)
			await SongsDatabase.CheckForData();

		song = SongsDatabase.GetSong(SongId)!;
	}
}