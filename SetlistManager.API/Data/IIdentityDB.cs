namespace SetlistManager.API.Data
{
    public interface IIdentityDB
    {
        Task TryLogInAsync();
        Task TryLogOutAsync();
        Task TryRegisterUserAsync();
    }
}