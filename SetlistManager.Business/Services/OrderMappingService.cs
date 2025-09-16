using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;
using SetlistManger.Business.Mappers;

namespace SetlistManger.Business.Services;

public class OrderMappingService
{
    private readonly AppDbContext _dbContext;
    private readonly ILanguageService _languageService;

    public OrderMappingService(AppDbContext dbContext, ILanguageService languageService)
    {
        _dbContext = dbContext;
        _languageService = languageService;
    }

    public async Task<SetlistModel> MapSongEntityToModelOrder(Setlist setlist)
    {
        var setlistModel = setlist.ToModel();

        foreach (var songSetlist in setlist.SongsSetlists)
        {
            setlistModel.Songs.First(s => s.Id == songSetlist.SongId).Order = songSetlist.Order;
        }
        
        setlistModel.Songs = setlistModel.Songs.OrderBy(x => x.Order).ToList();

        var languages =  await _languageService.GetAvailableLanguagesAsync();

        setlistModel.Songs = setlistModel.Songs.Select(song => 
        {
            song.Language = languages.First(x => x.Id == song.LanguageId);
            return song;
        }).ToList();

        return setlistModel;
    }

    public Setlist MapSongModelToEntity(SetlistModel setlistModel, Setlist setlist)
    {   
        foreach (var songModel in setlistModel.Songs)
        {
            var songEntity = _dbContext.Songs.FirstOrDefault(s => s.Id == songModel.Id);
            if (songEntity != null)
            {
                setlist.SongsSetlists.Add(new SongsSetlists
                {
                    SongId = songEntity.Id,
                    SetlistId = setlist.Id,
                    Order = songModel.Order
                });
            }
        }

        return setlist;
    }
}