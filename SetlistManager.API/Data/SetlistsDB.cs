using Dapper;
using Microsoft.OpenApi.Validations;
using SetlistManager.API.Entities;
using SetlistManager.Common.Models;
namespace SetlistManager.API.Data;
public class SetlistsDB(SqlConnectionFactory sqlConnectionFactory) : ISetlistsDB
{
    public async Task<SetlistModel?> GetSetlistById(string id)
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        
        const string sql = """
                SELECT Top(1)* FROM Setlists s
                JOIN SongsSetlists sl ON sl.SetlistId = s.Id
                WHERE s.Id = @Id;
            """;

        //setlist
        //relace mezi set a songy
        //detaily songu

        //pospojovani

        var x = await connection.QueryAsync<SetlistModel, SongModel, SetlistModel>(sql, (x, y) => { return x; }, new { Id = id }, splitOn: "SongId");
        return x.FirstOrDefault();
    }
    public async Task SaveSetlist(SetlistModel setlistModel)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        string sql = """
                INSERT INTO Setlists (Name)
                VALUES
                (@Name)
            """;

        await connection.ExecuteAsync(sql, new
        {
            setlistModel.Name
        });

        sql = """
                SELECT Top(1) * FROM Setlists ORDER BY Id DESC;
            """;
        var addedSetlist = await connection.QuerySingleOrDefaultAsync<SetlistModel>(sql);        

        List<object> songs = [];

        foreach (var songsInSetlist in setlistModel.Songs)
        {
            songs.Add(new { SongId = songsInSetlist.Id, SetlistId = addedSetlist.Id });
        }

        sql = """
                INSERT INTO SongsSetlists (SongId, SetlistId)
                VALUES
                (@SongId, @SetlistId)
            """;

        await connection.ExecuteAsync(sql, songs);
    }
}