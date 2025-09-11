using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SetlistManager.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, Role, int>(options)
{
    public DbSet<Instrument> Instruments { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Setlist> Setlists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<SongsSetlists> SongsSetlists { get; set; }
}