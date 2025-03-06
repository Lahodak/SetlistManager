using SetlistManager.Common.Models;

namespace SetlistManager.API.Data;

public class RoomsDB (SqlConnectionFactory sqlConnectionFactory) : IRoomsDB
{
    public async Task<int> CreateRoomAsync (JammingRoomModel room)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        string sql = """

            """;


        return 0;
    }
    
    public async Task<JammingRoomModel> JoinRoomAsync (int id)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        string sql = """

            """;

        return new();
    }
}