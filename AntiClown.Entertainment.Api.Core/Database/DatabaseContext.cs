using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AntiClown.Entertainment.Api.Core.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(
        DbContextOptions<DatabaseContext> options,
        IOptions<DatabaseOptions> dbOptionsAccessor
    ) : base(options)
    {
        Options = dbOptionsAccessor.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Options.ConnectionString);
    }

    public DbSet<CommonEventStorageElement> CommonEvents { get; set; }
    public DbSet<RaceDriverStorageElement> RaceDrivers { get; set; }
    public DbSet<RaceTrackStorageElement> RaceTracks { get; set; }

    private DatabaseOptions Options { get; }
}