using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;
using AntiClown.Entertainment.Api.Core.Parties.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Core.Contexts;
using SqlRepositoryBase.Core.Options;

namespace AntiClown.Entertainment.Api.Core.Database;

public class DatabaseContext(
    IConnectionStringProvider connectionStringProvider,
    IOptions<AppSettingsDatabaseOptions> appSettingsDatabaseOptions,
    ILogger<DatabaseContext> logger
) : PostgreSqlDbContext(connectionStringProvider, appSettingsDatabaseOptions, logger)
{
    public DbSet<CommonEventStorageElement> CommonEvents { get; set; }
    public DbSet<RaceDriverStorageElement> RaceDrivers { get; set; }
    public DbSet<RaceTrackStorageElement> RaceTracks { get; set; }
    public DbSet<ActiveEventsIndexStorageElement> ActiveEventsIndex { get; set; }
    public DbSet<DailyEventStorageElement> DailyEvents { get; set; }
    public DbSet<DailyActiveEventsIndexStorageElement> ActiveDailyEventsIndex { get; set; }
    public DbSet<PartyStorageElement> Parties { get; set; }
    public DbSet<MinecraftAccountStorageElement> MinecraftAccounts { get; set; }
    public DbSet<F1BingoCardStorageElement> F1BingoCards { get; set; }
    public DbSet<F1BingoBoardStorageElement> F1BingoBoards { get; set; }
}
