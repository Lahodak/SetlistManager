using Microsoft.EntityFrameworkCore;
using SetlistManager.Data;
using System.Text;

namespace SetlistManager.Business.Services.Implementations;

public class RoomCodeService : IRoomCodeService
{
    private const string roomCodeAvailableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int roomCodeLength = 6;
    private readonly AppDbContext _dbContext;

    public RoomCodeService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateUniqueRoomCodeAsync()
    {
        StringBuilder code = new(roomCodeLength);

        do
        {
            code.Clear();
            for (int i = 0; i < roomCodeLength; i++)
            {
                int index = Random.Shared.Next(roomCodeAvailableCharacters.Length - 1);
                code.Append(roomCodeAvailableCharacters[index]);
            }
        }
        while (await _dbContext.Rooms.AnyAsync(x => x.Code == code.ToString()));

        return code.ToString();
    }
}