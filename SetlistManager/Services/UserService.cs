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
            Name = "Jan Sáček",
            Username = "swegysacek31",
            Id = 1,
            Instruments = { InstrumentModel.Singer, InstrumentModel.Guitar }
        };

        UserModel user2 = new()
        {
            Name = "Žonza Háček",
            Username = "vyznamenanai31",
            Id = 1,
            Instruments = { InstrumentModel.Accordionist }
        };

        UserModel user3 = new()
        {
            Name = "Jan Skáček",
            Username = "namestimiru.com",
            Id = 1,
            Instruments = { InstrumentModel.Drummer }
        };

        SampleUsers.Add(user1);
        return SampleUsers;
    }
}