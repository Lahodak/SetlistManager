using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IRoomsService
{
    Task<RoomModel> CreateRoomAsync(RoomCreateModel createRoomModel, int hostId);
    Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel, User user);
    Task ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel);
    Task<RoomModel?> GetRoomByIdAsync(int roomId);
    Task<List<RoomModel>> GetPublicActiveRoomsAsync();
    Task<RoomModel?> GetRoomByCodeAsync(string roomCode);
}