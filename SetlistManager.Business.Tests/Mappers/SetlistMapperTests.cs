using SetlistManager.Business.Mappers;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Tests.Mappers;

public class SetlistMapperTests
{
    private static Song CreateSong(int id) => new()
    {
        Id = id,
        Name = $"Song {id}",
        ArtistId = 1,
        Artist = new Artist { Id = 1, Nick = "TestArtist", IsPublic = true, OwnerId = 42, Songs = [] },
        TabsURL = "",
        AudioURL = "",
        Tuning = "Standard",
        Key = "Am",
        BPM = 120,
        IsPublic = true,
        CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        OwnerId = 5,
        Owner = new User { Id = 5, UserName = "owner42", Email = "owner@test.com" },
        LanguageId = 1,
        Language = new Language { Id = 1, Name = "English", Code = "EN" }
    };

    private static SongsSetlists CreateSongsSetlists(int songId, int order) => new()
    {
        SongId = songId,
        Song = CreateSong(songId),
        SetlistId = 1,
        Order = order
    };

    private static Setlist CreateSetlist(List<SongsSetlists>? songsSetlists = null) => new()
    {
        Id = 1,
        Name = "Test Setlist",
        OwnerId = 42,
        Owner = new User { Id = 42, UserName = "owner42", Email = "owner@test.com" },
        SongsSetlists = songsSetlists ?? []
    };

    [Fact]
    public void ToModel_MapsAllProperties()
    {
        var setlist = CreateSetlist();

        var result = setlist.ToModel();

        Assert.Equal(setlist.Id, result.Id);
        Assert.Equal(setlist.Name, result.Name);
        Assert.Equal(setlist.OwnerId, result.Owner.Id);
    }

    [Fact]
    public void ToModel_WithSongs_MapsSongsInOrder()
    {
        var setlist = CreateSetlist([
            CreateSongsSetlists(songId: 3, order: 2),
            CreateSongsSetlists(songId: 1, order: 1),
            CreateSongsSetlists(songId: 2, order: 3)
        ]);

        var result = setlist.ToModel();

        Assert.Equal(3, result.Songs.Count);
        Assert.Equal(1, result.Songs[0].Id);
        Assert.Equal(3, result.Songs[1].Id);
        Assert.Equal(2, result.Songs[2].Id);
    }

    [Fact]
    public void ToModel_WithSongs_SetsOrderOnSongModels()
    {
        var setlist = CreateSetlist([
            CreateSongsSetlists(songId: 1, order: 5),
            CreateSongsSetlists(songId: 2, order: 10)
        ]);

        var result = setlist.ToModel();

        Assert.Equal(5, result.Songs[0].Order);
        Assert.Equal(10, result.Songs[1].Order);
    }

    [Fact]
    public void ToModel_MapsSongPropertiesCorrectly()
    {
        var setlist = CreateSetlist([CreateSongsSetlists(songId: 1, order: 1)]);

        var result = setlist.ToModel();

        var song = result.Songs[0];
        Assert.Equal("Song 1", song.Name);
        Assert.Equal("TestArtist", song.Artist.Nick);
        Assert.Equal("English", song.Language.Name);
    }
}