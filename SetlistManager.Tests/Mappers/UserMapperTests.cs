using SetlistManager.Business.Mappers;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Tests.Mappers;

public class UserMapperTests
{
    private static User CreateUser(Instrument? instrument = null, List<Token>? tokens = null) => new()
    {
        Id = 1,
        UserName = "testuser",
        Email = "test@example.com",
        Instrument = instrument,
        Tokens = tokens ?? []
    };

    private static Instrument CreateInstrument() => new()
    {
        Id = 3,
        Name = "Guitar"
    };

    private static Token CreateToken(string provider = "Spotify") => new()
    {
        Id = 1,
        AccessToken = "access_123",
        RefreshToken = "refresh_456",
        Provider = new Provider { Id = 1, Name = provider }
    };

    [Fact]
    public void ToModel_MapsAllScalarProperties()
    {
        var user = CreateUser();

        var result = user.ToModel();

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.UserName, result.Username);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public void ToModel_WithInstrument_MapsInstrument()
    {
        var instrument = CreateInstrument();
        var user = CreateUser(instrument: instrument);

        var result = user.ToModel();

        Assert.NotNull(result.Instrument);
        Assert.Equal(instrument.Id, result.Instrument.Id);
        Assert.Equal(instrument.Name, result.Instrument.Name);
    }

    [Fact]
    public void ToModel_WithNullInstrument_DefaultsToNoInstrument()
    {
        var user = CreateUser(instrument: null);

        var result = user.ToModel();

        Assert.NotNull(result.Instrument);
        Assert.Equal(0, result.Instrument.Id);
        Assert.Equal("No Instrument", result.Instrument.Name);
    }

    [Fact]
    public void ToModel_WithTokens_MapsTokens()
    {
        var user = CreateUser(tokens: [CreateToken("Spotify"), CreateToken("Apple Music")]);

        var result = user.ToModel();

        Assert.NotNull(result.Tokens);
        Assert.Equal(2, result.Tokens.Count);
        Assert.Equal("access_123", result.Tokens[0].AccessToken);
        Assert.Equal("refresh_456", result.Tokens[0].RefreshToken);
        Assert.Equal("Spotify", result.Tokens[0].Provider);
        Assert.Equal("Apple Music", result.Tokens[1].Provider);
    }

    [Fact]
    public void ToModel_WithNullTokens_ReturnsNullTokens()
    {
        var user = CreateUser();
        user.Tokens = null!;

        var result = user.ToModel();

        Assert.Null(result.Tokens);
    }

    [Fact]
    public void ToModel_WithEmptyTokens_ReturnsEmptyList()
    {
        var user = CreateUser(tokens: []);

        var result = user.ToModel();

        Assert.NotNull(result.Tokens);
        Assert.Empty(result.Tokens);
    }

    [Fact]
    public void ToViewModel_MapsAllProperties()
    {
        var user = CreateUser();

        var result = user.ToViewModel();

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.UserName, result.UserName);
        Assert.Equal(user.Email, result.Email);
    }
}