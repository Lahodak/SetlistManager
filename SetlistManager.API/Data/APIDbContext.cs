using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data.Entities;
using SetlistManager.API.Data.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SetlistManager.API.Data;

public class APIDbContext(DbContextOptions<APIDbContext> options) : IdentityDbContext<User, Role, int>(options)
{
    public DbSet<Instrument> Instruments { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Setlist> Setlists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<SongsSetlists> SongsSetlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}