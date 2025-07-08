using Dapper;
using SetlistManager.API;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Data;

public class SongsDB(SqlConnectionFactory sqlConnectionFactory) : ISongsDB
{
    public async Task<IEnumerable<SongModel>> GetSongsAsync()
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Songs;";
        return await connection.QueryAsync<SongModel>(sql);
    }

    public async Task<SongModel?> GetSongByIdAsync(int id)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
                SELECT * FROM Songs
                WHERE Id = @Id;
            """;

        return await connection.QuerySingleOrDefaultAsync<SongModel>(sql, new { Id = id });
    }

    public async Task UploadSongs(SongModel song)
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