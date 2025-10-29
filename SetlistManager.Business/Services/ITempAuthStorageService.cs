namespace SetlistManager.Business.Services;

public interface ITempAuthStorageService
{
    Task<string> CreateNewTempAuthSecret(int userId);
}