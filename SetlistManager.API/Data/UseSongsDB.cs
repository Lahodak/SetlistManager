using SetlistManager.API.Models;
using Dapper;
namespace SetlistManager.API.Data;

public class UseSongsDB(SqlConnectionFactory sqlConnectionFactory) : ISongsDB
{
    public async Task<IEnumerable<Song>>GetSongsAsync()
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        const string sql = "SELECT * FROM SongsCollection;";
        return await connection.QueryAsync<Song>(sql);
    }
    public async Task<Song?> GetSongByIdAsync(int id)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
                SELECT * FROM SongsCollection
                WHERE SongId = @SongId;
            """;

        return await connection.QuerySingleOrDefaultAsync<Song>(sql, new { SongId = id });
    }
}