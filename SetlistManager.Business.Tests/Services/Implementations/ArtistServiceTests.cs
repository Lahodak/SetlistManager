using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SetlistManager.Business.Services;
using SetlistManager.Business.Services.Implementations;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Tests.Services.Implementations;

public class ArtistServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly ArtistService _service;
    private const int CurrentUserId = 1;

    public ArtistServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);

        var currentUserContext = Substitute.For<ICurrentUserContext>();
        currentUserContext.UserId.Returns(CurrentUserId);

        _service = new ArtistService(_dbContext, currentUserContext);
    }

    public void Dispose() => _dbContext.Dispose();

    private async Task<Artist> SeedArtistAsync(string nick = "TestArtist", bool isPublic = false, int ownerId = CurrentUserId)
    {
        var artist = new Artist { Nick = nick, IsPublic = isPublic, OwnerId = ownerId };
        _dbContext.Artists.Add(artist);
        await _dbContext.SaveChangesAsync();
        return artist;
    }

    [Fact]
    public async Task TryCreateArtistAsync_CreatesArtist()
    {
        var model = new ArtistCreateModel { Nick = "NewArtist", IsPublic = false };

        await _service.CreateArtistAsync(model);

        var artist = await _dbContext.Artists.SingleAsync();
        Assert.Equal("NewArtist", artist.Nick);
        Assert.Equal(CurrentUserId, artist.OwnerId);
        Assert.False(artist.IsPublic);
    }

    [Fact]
    public async Task TryCreateArtistAsync_WithDuplicateNick_ThrowsDuplicateEntry()
    {
        await SeedArtistAsync(nick: "Duplicate");
        var model = new ArtistCreateModel { Nick = "Duplicate" };

        await Assert.ThrowsAsync<DuplicateEntryException>(
            () => _service.CreateArtistAsync(model));
    }

    [Fact]
    public async Task GetArtistByIdAsync_OwnedArtist_ReturnsModel()
    {
        var artist = await SeedArtistAsync();

        var result = await _service.GetArtistByIdAsync(artist.Id, ContentType.Private);

        Assert.NotNull(result);
        Assert.Equal(artist.Nick, result.Nick);
    }

    [Fact]
    public async Task GetArtistByIdAsync_NotOwnedPrivateArtist_ThrowsEntryNotFound()
    {
        var artist = await SeedArtistAsync(ownerId: 999);

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.GetArtistByIdAsync(artist.Id, ContentType.Private));
    }

    [Fact]
    public async Task GetArtistByIdAsync_PublicArtist_ReturnsModel()
    {
        var artist = await SeedArtistAsync(isPublic: true);

        var result = await _service.GetArtistByIdAsync(artist.Id, ContentType.Public);

        Assert.NotNull(result);
        Assert.Equal(artist.Nick, result.Nick);
    }

    [Fact]
    public async Task TryUpdateArtistAsync_OwnedArtist_UpdatesNick()
    {
        var artist = await SeedArtistAsync(nick: "OldNick");
        var model = new ArtistUpdateModel { Nick = "NewNick" };

        await _service.UpdateArtistAsync(artist.Id, model);

        var updated = await _dbContext.Artists.FindAsync(artist.Id);
        Assert.Equal("NewNick", updated!.Nick);
    }

    [Fact]
    public async Task TryUpdateArtistAsync_NickConflict_ThrowsEntryNotFound()
    {
        await SeedArtistAsync(nick: "Existing", ownerId: 999);
        var artist = await SeedArtistAsync(nick: "Mine");
        var model = new ArtistUpdateModel { Nick = "Existing" };

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.UpdateArtistAsync(artist.Id, model));
    }

    [Fact]
    public async Task TryUpdateArtistAsync_NotOwned_ThrowsEntryNotFound()
    {
        var artist = await SeedArtistAsync(ownerId: 999);
        var model = new ArtistUpdateModel { Nick = "Updated" };

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.UpdateArtistAsync(artist.Id, model));
    }

    [Fact]
    public async Task TryDeleteArtistAsync_OwnedPrivateArtist_Deletes()
    {
        var artist = await SeedArtistAsync();

        await _service.DeleteArtistAsync(artist.Id);

        Assert.Empty(await _dbContext.Artists.ToListAsync());
    }

    [Fact]
    public async Task TryDeleteArtistAsync_PublicArtist_ThrowsEntryNotFound()
    {
        var artist = await SeedArtistAsync(isPublic: true);

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.DeleteArtistAsync(artist.Id));
    }

    [Fact]
    public async Task TryDeleteArtistAsync_NotOwned_ThrowsEntryNotFound()
    {
        var artist = await SeedArtistAsync(ownerId: 999);

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.DeleteArtistAsync(artist.Id));
    }

    [Fact]
    public async Task TryMakeArtistPublicAsync_OwnedArtist_SetsPublic()
    {
        var artist = await SeedArtistAsync();

        await _service.MakeArtistPublicAsync(artist.Id);

        var updated = await _dbContext.Artists.FindAsync(artist.Id);
        Assert.True(updated!.IsPublic);
    }

    [Fact]
    public async Task TryMakeArtistPublicAsync_NotOwned_ThrowsEntryNotFound()
    {
        var artist = await SeedArtistAsync(ownerId: 999);

        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.MakeArtistPublicAsync(artist.Id));
    }

    [Fact]
    public async Task TryGiveAccessToUserAsync_OwnedArtist_CreatesArtistsUsers()
    {
        var artist = await SeedArtistAsync();
        int targetUserId = 50;

        await _service.GiveAccessToUserAsync(artist.Id, targetUserId);

        var entry = await _dbContext.ArtistsUsers.SingleAsync();
        Assert.Equal(artist.Id, entry.ArtistId);
        Assert.Equal(targetUserId, entry.UserId);
    }

    [Fact]
    public async Task TryGiveAccessToUserAsync_NonexistentArtist_ThrowsEntryNotFound()
    {
        await Assert.ThrowsAsync<EntryNotFoundException>(
            () => _service.GiveAccessToUserAsync(999, 50));
    }
}