using Dapper;
using SetlistManager.Common.Models;
namespace SetlistManager.API.Data;
public class SetlistsDB(SqlConnectionFactory sqlConnectionFactory) : ISetlistsDB
{
    public async Task<SetlistModel?> GetSetlistById(int id)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        SetlistModel? setlistResult = null;

        string sql = """
                SELECT s.Id, s.Name,
                   song.Id, song.Name, song.Artist, song.Language, song.TabsURL, song.YouTubeURL
            FROM Setlists s
            JOIN SongsSetlists sl ON sl.SetlistId = s.Id
            JOIN Songs song ON song.Id = sl.SongId
            WHERE s.Id = @Id;
            """;

        var x = await connection.QueryAsync<SetlistModel, SongModel, SetlistModel>(sql, 
            (setlist, song) => 
            {
                if (setlistResult == null)
                {
                    setlistResult = setlist;
                    setlistResult.Songs = [];
                }

                setlistResult.Songs.Add(song);
                
                return setlistResult; 
            }, 
            new { Id = id }, 
            splitOn: "Id");        
            
        return setlistResult;
    }

    public async Task<int> SaveSetlist(SetlistModel setlistModel)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        string sql = """
                INSERT INTO Setlists (Name)
                OUTPUT INSERTED.Id
                VALUES (@Name);
            """;

        int id = await connection.QuerySingleAsync<int>(sql, new
        {
                setlistModel.Name   
        });               

        List<object> songs = [];

        foreach (var songsInSetlist in setlistModel.Songs)
        {
            songs.Add(new { SongId = songsInSetlist.Id, SetlistId = id });
        }

        sql = """
                INSERT INTO SongsSetlists (SongId, SetlistId)
                VALUES
                (@SongId, @SetlistId)
            """;

        await connection.ExecuteAsync(sql, songs);
        return id;
    }
}