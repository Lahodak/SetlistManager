using SetlistManager.Common.Models;

namespace SetlistManager.Services;
public class UserService
{
    private readonly IHttpClientFactory _httpClientFactory; 
    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public void SaveUser(UserModel userModel)
    {
        if (userModel == null)
        {
            return;
        }

        var userToBeSaved = userModel;
    }

    public List<UserModel> GetUsers()
    {
        List<UserModel> SampleUsers = [];

        UserModel user1 = new()
        {
            Id = 1,
            Username = "Sitma",
            Instrument = InstrumentModel.Singer
        };

        UserModel user2 = new()
        {
            Username = "vyznamenanai31",
            Id = 2,
            Instrument = InstrumentModel.Accordionist
        };

        UserModel user3 = new()
        {
            Username = "namestimiru.com",
            Id = 3,
            Instrument = InstrumentModel.Drummer
        };
        

        SampleUsers.Add(user1);
        return SampleUsers;
    }
}