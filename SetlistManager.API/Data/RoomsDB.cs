using SetlistManager.Common.Models;

namespace SetlistManager.API.Data;

public class RoomsDB(SqlConnectionFactory sqlConnectionFactory) : IRoomsDB
{
    public async Task<int> CreateRoomAsync(RoomModel room)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        string sql = """

            """;


        return 0;
    }

    public async Task<RoomModel> JoinRoomAsync(int id, UserModel user)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        string sql = """

            """;

        return new();
    }

    public async Task<int> ChangeCurrentSongAsync(int roomId)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        string sql = """

            """;

        return 0;
    }
}