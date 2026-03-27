namespace SetlistManager.Business.Services;

/// <summary>
/// Defines a contract for generating unique room codes for identifying rooms within the system.
/// </summary>
public interface IRoomCodeService
{
    /// <summary>
    /// Generates a unique code for identifying a room.
    /// </summary>
    /// <returns>The task result contains a string that is the unique room code.</returns>
    Task<string> GenerateUniqueRoomCodeAsync();
}