using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;
using AntiClown.Entertainment.Api.Core.Options;
using AntiClown.Entertainment.Api.Core.Parties.Repositories;
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
    public DbSet<ActiveEventsIndexStorageElement> ActiveEventsIndex { get; set; }
    public DbSet<DailyEventStorageElement> DailyEvents { get; set; }
    public DbSet<DailyActiveEventsIndexStorageElement> ActiveDailyEventsIndex { get; set; }
    public DbSet<PartyStorageElement> Parties { get; set; }
    public DbSet<F1RaceStorageElement> F1PredictionsRaces { get; set; }
    public DbSet<F1PredictionResultStorageElement> F1PredictionsResults { get; set; }

    private DatabaseOptions Options { get; }
}