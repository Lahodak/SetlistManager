using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data.Entities;
using SetlistManager.API.Data.Configuration;

namespace SetlistManager.API.Data;

public class APIDbContext(DbContextOptions<APIDbContext> options) : DbContext(options)
{
    public DbSet<Instrument> Instruments { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Setlist> Setlists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RoomsSetlists> RoomsSetlists { get; set; }
    public DbSet<SongsSetlists> SongsSetlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new InstrumentConfig().Configure(modelBuilder.Entity<Instrument>());
        new LanguageConfig().Configure(modelBuilder.Entity<Language>());
        new RoomConfig().Configure(modelBuilder.Entity<Room>());
        new SetlistConfig().Configure(modelBuilder.Entity<Setlist>());
        new SongConfig().Configure(modelBuilder.Entity<Song>());
        new UserConfig().Configure(modelBuilder.Entity<User>());
        new RoomsSetlistsConfig().Configure(modelBuilder.Entity<RoomsSetlists>());
        new SongsSetlistsConfig().Configure(modelBuilder.Entity<SongsSetlists>());
    }
}