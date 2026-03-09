using SetlistManager.Business.Mappers;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Tests.Mappers;

public class SongMapperTests
{
    private static Language CreateLanguage() => new()
    {
        Id = 1,
        Name = "English",
        Code = "EN"
    };

    private static Artist CreateArtist() => new()
    {
        Id = 10,
        Nick = "TestArtist",
        IsPublic = true,
        OwnerId = 42,
        Songs = []
    };

    private static User CreateOwner() => new()
    {
        Id = 5,
        UserName = "owner42",
        Email = "owner@test.com"
    };

    private static Song CreateSong(Artist? artist = null, Language? language = null, User? owner = null) => new()
    {
        Id = 1,
        Name = "Test Song",
        ArtistId = 10,
        Artist = artist ?? CreateArtist(),
        TabsURL = "https://tabs.example.com",
        AudioURL = "https://audio.example.com",
        Tuning = "Standard",
        Key = "Am",
        BPM = 120,
        IsPublic = true,
        CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
        OwnerId = 5,
        Owner = owner ?? CreateOwner(),
        LanguageId = 1,
        Language = language ?? CreateLanguage()
    };

    [Fact]
    public void ToModel_MapsAllScalarProperties()
    {
        var song = CreateSong();

        var result = song.ToModel();

        Assert.Equal(song.Id, result.Id);
        Assert.Equal(song.Name, result.Name);
        Assert.Equal(song.TabsURL, result.TabsURL);
        Assert.Equal(song.AudioURL, result.AudioURL);
        Assert.Equal(song.Tuning, result.Tuning);
        Assert.Equal(song.Key, result.Key);
        Assert.Equal(song.BPM, result.BPM);
        Assert.Equal(song.IsPublic, result.IsPublic);
        Assert.Equal(song.CreatedAt, result.CreatedAt);
        Assert.Equal(song.UpdatedAt, result.UpdatedAt);
        Assert.Equal(song.OwnerId, result.OwnerId);
        Assert.Equal(song.LanguageId, result.LanguageId);
    }

    [Fact]
    public void ToModel_MapsArtistWithoutSongs()
    {
        var artist = CreateArtist();
        var song = CreateSong(artist: artist);

        var result = song.ToModel();

        Assert.Equal(artist.Id, result.Artist.Id);
        Assert.Equal(artist.Nick, result.Artist.Nick);
        Assert.Null(result.Artist.Songs);
    }

    [Fact]
    public void ToModel_MapsLanguage()
    {
        var language = CreateLanguage();
        var song = CreateSong(language: language);

        var result = song.ToModel();

        Assert.Equal(language.Id, result.Language.Id);
        Assert.Equal(language.Name, result.Language.Name);
        Assert.Equal(language.Code, result.Language.Code);
    }

    [Fact]
    public void ToModel_MapsOwnerNick()
    {
        var owner = CreateOwner();
        var song = CreateSong(owner: owner);

        var result = song.ToModel();

        Assert.Equal("owner42", result.OwnerNick);
    }

    [Fact]
    public void ToModel_WithNullOwner_ReturnsNullOwnerNick()
    {
        var song = CreateSong();
        song.Owner = null!;

        var result = song.ToModel();

        Assert.Null(result.OwnerNick);
    }

    [Fact]
    public void ToModelWithoutArtist_MapsAllScalarProperties()
    {
        var song = CreateSong();

        var result = song.ToModelWithoutArtist();

        Assert.Equal(song.Id, result.Id);
        Assert.Equal(song.Name, result.Name);
        Assert.Equal(song.TabsURL, result.TabsURL);
        Assert.Equal(song.AudioURL, result.AudioURL);
        Assert.Equal(song.Tuning, result.Tuning);
        Assert.Equal(song.Key, result.Key);
        Assert.Equal(song.BPM, result.BPM);
        Assert.Equal(song.IsPublic, result.IsPublic);
        Assert.Equal(song.OwnerId, result.OwnerId);
    }

    [Fact]
    public void ToModelWithoutArtist_DoesNotMapArtist()
    {
        var song = CreateSong();

        var result = song.ToModelWithoutArtist();

        Assert.Equal(0, result.Artist.Id);
    }

    [Fact]
    public void ToEntity_MapsAllFieldsFromCreateModel()
    {
        var model = new SongCreateModel
        {
            Name = "New Song",
            ArtistId = 10,
            LanguageId = 1,
            TabsURL = "https://tabs.example.com",
            AudioURL = "https://audio.example.com",
            Tuning = "Drop D",
            Key = "E",
            BPM = 140
        };

        var result = model.ToEntity(creatorId: 5, isArtistPublic: true);

        Assert.Equal(model.Name, result.Name);
        Assert.Equal(model.ArtistId, result.ArtistId);
        Assert.Equal(model.LanguageId, result.LanguageId);
        Assert.Equal(model.TabsURL, result.TabsURL);
        Assert.Equal(model.AudioURL, result.AudioURL);
        Assert.Equal(model.Tuning, result.Tuning);
        Assert.Equal(model.Key, result.Key);
        Assert.Equal(model.BPM, result.BPM);
        Assert.Equal(5, result.OwnerId);
        Assert.True(result.IsPublic);
    }

    [Fact]
    public void ToEntity_SetsCreatedAtToUtcNow()
    {
        var model = new SongCreateModel
        {
            Name = "New Song",
            ArtistId = 10,
            LanguageId = 1,
            TabsURL = "",
            AudioURL = "",
            Tuning = "Standard",
            Key = "C",
            BPM = 100
        };
        var before = DateTime.UtcNow;

        var result = model.ToEntity(creatorId: 1, isArtistPublic: false);

        var after = DateTime.UtcNow;
        Assert.InRange(result.CreatedAt, before, after);
    }

    [Fact]
    public void UpdateEntity_UpdatesAllMutableFields()
    {
        var song = CreateSong();
        var updateModel = new SongUpdateModel
        {
            Name = "Updated Song",
            ArtistId = 20,
            LanguageId = 2,
            TabsURL = "https://newtabs.example.com",
            AudioURL = "https://newaudio.example.com",
            Tuning = "Drop C",
            Key = "D",
            BPM = 180
        };
        var before = DateTime.UtcNow;

        song.UpdateEntity(updateModel);

        var after = DateTime.UtcNow;
        Assert.Equal("Updated Song", song.Name);
        Assert.Equal(20, song.ArtistId);
        Assert.Equal(2, song.LanguageId);
        Assert.Equal("https://newtabs.example.com", song.TabsURL);
        Assert.Equal("https://newaudio.example.com", song.AudioURL);
        Assert.Equal("Drop C", song.Tuning);
        Assert.Equal("D", song.Key);
        Assert.Equal(180, song.BPM);
        Assert.NotNull(song.UpdatedAt);
        Assert.InRange(song.UpdatedAt.Value, before, after);
    }
}