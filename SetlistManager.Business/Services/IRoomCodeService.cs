namespace SetlistManager.Business.Services;

public interface IRoomCodeService
{
    Task<string> GenerateUniqueRoomCodeAsync();
}