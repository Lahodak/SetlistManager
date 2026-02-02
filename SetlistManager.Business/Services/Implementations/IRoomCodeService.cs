namespace SetlistManager.Business.Services.Implementations;

public interface IRoomCodeService
{
    Task<string> GenerateUniqueRoomCodeAsync();
}