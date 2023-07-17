using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(
        DbContextOptions<DatabaseContext> dbContextOptions,
        IOptions<DatabaseOptions> databaseOptions
    ) : base(dbContextOptions)
    {
        Options = databaseOptions.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Options.ConnectionString);
    }
    
    public DbSet<InteractivityStorageElement> Interactivity { get; set; }

    private DatabaseOptions Options { get; }
}