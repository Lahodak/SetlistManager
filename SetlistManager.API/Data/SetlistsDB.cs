using Dapper;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Common.Models;
namespace SetlistManager.API.Data;
public class SetlistsDB: ISetlistsDB
{
    private readonly APIDbContext _dbContext;
    public SetlistsDB(APIDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SetlistModel?> GetSetlistById(int id)
    {
        var setlist = _dbContext.Setlists    
            .Include(s => s.SongsSetlists)
            .ThenInclude(s => s.Song)
            .FirstOrDefaultAsync(x => x.Id == id);

        return new();
    }

    public async Task<int> SaveSetlist(SetlistModel setlistModel)
    {
        
        return 0;
    }
}