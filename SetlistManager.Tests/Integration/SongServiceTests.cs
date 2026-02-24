using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SetlistManager.Business.Services;
using SetlistManager.Business.Services.Implementations;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Tests.Integration;

public class SongServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly SongService _service;
    private const int CurrentUserId = 1;

    public SongServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);

        var currentUserContext = Substitute.For<ICurrentUserContext>();
        currentUserContext.UserId.Returns(CurrentUserId);

        _service = new SongService(_dbContext, currentUserContext);
    }

    public void Dispose() => _dbContext.Dispose();

    private async Task<Artist> SeedArtistAsync(bool isPublic = false, int ownerId = CurrentUserId)
    {
        var artist = new Artist { Nick = "TestArtist", IsPublic = isPublic, OwnerId = ownerId };
        _dbContext.Artists.Add(artist);
        await _dbContext.SaveChangesAsync();
        return artist;
    }

    private async Task<Song> SeedSongAsync(Artist? artist = null, string name = "Test Song", bool isPublic = false, int ownerId = CurrentUserId)
    {
        artist ??= await SeedArtistAsync();

        var language = new Language { Name = "English", Code = "EN" };
        _dbContext.Languages.Add(language);

        var owner = new User { Id = ownerId, UserName = "owner", Email = "owner@test.com" };

        if (!await _dbContext.Users.AnyAsync(u => u.Id == ownerId))
            _dbContext.Users.Add(owner);

        var song = new Song
        {
            Name = name,
            ArtistId = artist.Id,
            TabsURL = "",
            AudioURL = "",
            Tuning = "Standard",
            Key = "Am",
            BPM = 120,
            IsPublic = isPublic,
            CreatedAt = DateTime.UtcNow,
            OwnerId = ownerId,
            Language = language
        };
        _dbContext.Songs.Add(song);
        await _dbContext.SaveChangesAsync();
        return song;
    }

    [Fact]
    public async Task TryCreateSongAsync_CreatesSong()
    {
        var artist = await SeedArtistAsync();
        var model = new SongCreateModel
        {
            Name = "New Song",
            ArtistId = artist.Id,
            LanguageId = 1,
            TabsURL = "",
            AudioURL = "",
            Tuning = "Standard",
            Key = "C",
            BPM = 120
        };

        _dbContext.Languages.Add(new Language { Id = 1, Name = "English", Code = "EN" });
        await _dbContext.SaveChangesAsync();

        await _service.CreateSongAsync(model);

        var song = await _dbContext.Songs.SingleAsync(s => s.Name == "New Song");
        Assert.Equal(CurrentUserId, song.OwnerId);
        Assert.Equal(artist.Id, song.ArtistId);
    }

    [Fact]
    public async Task TryCreateSongAsync_DuplicateNameAndArtist_ThrowsDuplicateEntry()
    {
        var artist = await SeedArtistAsync();
        await SeedSongAsync(artist: artist, name: "Duplicate");

        var model = new SongCreateModel
        {
            Name = "Duplicate",
            ArtistId = artist.Id,
            LanguageId = 1,
            TabsURL = "",
            AudioURL = "",
            Tuning = "Standard",
            Key = "C",
            BPM = 120
        };

        await Assert.ThrowsAsync<DuplicateEntryException>(
            () => _service.CreateSongAsync(model));
    }

    [Fact]
    public async Task GetSongByIdAsync_OwnedSong_ReturnsModel()
    {
        var song = await SeedSongAsync();

        var result = await _service.GetSongByIdAsync(song.Id);

        Assert.NotNull(result);
        Assert.Equal(song.Name, result.Name);
    }

    [Fact]
    public async Task GetSongByIdAsync_PublicSong_ReturnsModel()
    {
        var song = await SeedSongAsync(isPublic: true, ownerId: 999);

        var result = await _service.GetSongByIdAsync(song.Id);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetSongByIdAsync_PrivateNotOwned_ReturnsNull()
    {
        var song = await SeedSongAsync(ownerId: 999);

        var result = await _service.GetSongByIdAsync(song.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSongByIdAsync_Nonexistent_ReturnsNull()
    {
        var result = await _service.GetSongByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task TryUpdateSongAsync_OwnedPrivateSong_Updates()
    {
        var song = await SeedSongAsync();
        var model = new SongUpdateModel
        {
            Name = "Updated",
            ArtistId = song.ArtistId,
            LanguageId = song.LanguageId,
            TabsURL = "https://newtabs.example.com",
            AudioURL = "",
            Tuning = "Drop D",
            Key = "E",
            BPM = 140
        };

        await _service.UpdateSongAsync(song.Id, model);

        var updated = await _dbContext.Songs.FindAsync(song.Id);
        Assert.Equal("Updated", updated!.Name);
        Assert.Equal("Drop D", updated.Tuning);
        Assert.Equal(140, updated.BPM);
    }

    [Fact]
    public async Task TryUpdateSongAsync_PublicSong_ThrowsEntryNotFound()
    {
        var song = await SeedSongAsync(isPublic: true);
        var model = new SongUpdateModel
        {
            Name = "Updated",
            ArtistId = song.ArtistId,
            LanguageId = song.LanguageId,
            TabsURL = "",
            AudioURL = "",
            Tuning = "Standard",
            Key = "Am",
            BPM = 120
        };

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.UpdateSongAsync(song.Id, model));
    }

    [Fact]
    public async Task TryUpdateSongAsync_NotOwned_ThrowsEntryNotFound()
    {
        var song = await SeedSongAsync(ownerId: 999);
        var model = new SongUpdateModel
        {
            Name = "Updated",
            ArtistId = song.ArtistId,
            LanguageId = song.LanguageId,
            TabsURL = "",
            AudioURL = "",
            Tuning = "Standard",
            Key = "Am",
            BPM = 120
        };

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.UpdateSongAsync(song.Id, model));
    }

    [Fact]
    public async Task TryDeleteSongAsync_OwnedSong_Deletes()
    {
        var song = await SeedSongAsync();

        await _service.DeleteSongAsync(song.Id);

        Assert.False(await _dbContext.Songs.AnyAsync(s => s.Id == song.Id));
    }

    [Fact]
    public async Task TryDeleteSongAsync_NotOwned_ThrowsEntryNotFound()
    {
        var song = await SeedSongAsync(ownerId: 999);

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.DeleteSongAsync(song.Id));
    }

    [Fact]
    public async Task TryDeleteSongAsync_Nonexistent_ThrowsEntryNotFound()
    {
        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.DeleteSongAsync(999));
    }

    [Fact]
    public async Task TryMakeSongPublicAsync_OwnedSong_SetsSongAndArtistPublic()
    {
        var artist = await SeedArtistAsync(isPublic: false);
        var song = await SeedSongAsync(artist: artist);

        await _service.MakeSongPublicAsync(song.Id);

        var updatedSong = await _dbContext.Songs.FindAsync(song.Id);
        var updatedArtist = await _dbContext.Artists.FindAsync(artist.Id);
        Assert.True(updatedSong!.IsPublic);
        Assert.True(updatedArtist!.IsPublic);
    }

    [Fact]
    public async Task TryMakeSongPublicAsync_NotOwned_ThrowsEntryNotFound()
    {
        var song = await SeedSongAsync(ownerId: 999);

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.MakeSongPublicAsync(song.Id));
    }
}