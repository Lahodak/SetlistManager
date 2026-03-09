namespace SetlistManager.Business.Services;

/// <summary>
/// Defines methods for managing temporary authentication secrets associated with users. Used as the State parameter in Genius Authorization flow.
/// </summary>
public interface ITempAuthStorageService
{
    /// <summary>
    /// Generates a new temporary authentication secret for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the temporary authentication secret is generated. </param>
    /// <returns>Returns the newly generated temporary authentication secret as a string.</returns>
    Task<string> CreateNewTempAuthSecret(int userId);
}