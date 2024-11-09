using Dapper;
using SetlistManager.API.Entities;
namespace SetlistManager.API.Data;

public class SongsDB(SqlConnectionFactory sqlConnectionFactory) : ISongsDB
{
    public async Task<IEnumerable<Song>>GetSongsAsync()
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Songs;";
        return await connection.QueryAsync<Song>(sql);
    }
    public async Task<Song?> GetSongByIdAsync(int id)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
                SELECT * FROM Songs
                WHERE Id = @Id;
            """;

        return await connection.QuerySingleOrDefaultAsync<Song>(sql, new { Id = id });
    }
    public async Task UploadSongs(Song song)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
                INSERT INTO Songs (Name, Artist, Language, TabsURL, YouTubeURL)
                VALUES
                (@Name, @Artist, @Language, @TabsURL, @YouTubeURL)
            """;
        await connection.ExecuteAsync(sql, new 
        {
            song.Name,
            song.Artist,
            song.Language,
            song.TabsURL,
            song.YouTubeURL
        });
    }
}