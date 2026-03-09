using SetlistManager.Business.Mappers;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Tests.Mappers;

public class ArtistMapperTests
{
    private static Artist CreateArtist(List<Song>? songs = null) => new()
    {
        Id = 1,
        Nick = "TestArtist",
        IsPublic = true,
        OwnerId = 42,
        Songs = songs ?? []
    };

    private static Song CreateSong(int id) => new()
    {
        Id = id,
        Name = $"Song {id}",
        ArtistId = 1,
        Language = new Language { Id = 1, Name = "English", Code = "EN" }
    };

    [Fact]
    public void ToModel_MapsAllScalarProperties()
    {
        var artist = CreateArtist();

        var result = artist.ToModel();

        Assert.Equal(artist.Id, result.Id);
        Assert.Equal(artist.Nick, result.Nick);
        Assert.Equal(artist.IsPublic, result.IsPublic);
        Assert.Equal(artist.OwnerId, result.OwnerId);
    }

    [Fact]
    public void ToModel_WithSongs_MapsSongsToModels()
    {
        var artist = CreateArtist([CreateSong(1), CreateSong(2)]);

        var result = artist.ToModel();

        Assert.NotNull(result.Songs);
        Assert.Equal(2, result.Songs.Count);
        Assert.Equal("Song 1", result.Songs[0].Name);
        Assert.Equal("Song 2", result.Songs[1].Name);
    }

    [Fact]
    public void ToModel_WithNullSongs_ReturnsNullSongs()
    {
        var artist = CreateArtist(null!);
        artist.Songs = null!;

        var result = artist.ToModel();

        Assert.Null(result.Songs);
    }

    [Fact]
    public void ToModel_WithEmptySongs_ReturnsEmptyList()
    {
        var artist = CreateArtist([]);

        var result = artist.ToModel();

        Assert.NotNull(result.Songs);
        Assert.Empty(result.Songs);
    }

    [Fact]
    public void ToModelWithoutSongs_MapsAllScalarProperties()
    {
        var artist = CreateArtist([CreateSong(1)]);

        var result = artist.ToModelWithoutSongs();

        Assert.Equal(artist.Id, result.Id);
        Assert.Equal(artist.Nick, result.Nick);
        Assert.Equal(artist.IsPublic, result.IsPublic);
        Assert.Equal(artist.OwnerId, result.OwnerId);
    }

    [Fact]
    public void ToModelWithoutSongs_AlwaysReturnsNullSongs()
    {
        var artist = CreateArtist([CreateSong(1), CreateSong(2)]);

        var result = artist.ToModelWithoutSongs();

        Assert.Null(result.Songs);
    }
}