using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Releases.Repositories;
using AntiClown.DiscordBot.Roles.Repositories;
using AntiClown.DiscordBot.Utility.Locks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Core.Contexts;
using SqlRepositoryBase.Core.Options;

namespace AntiClown.DiscordBot.Database;

public class DatabaseContext(
    IConnectionStringProvider connectionStringProvider,
    IOptions<AppSettingsDatabaseOptions> appSettingsDatabaseOptions,
    ILogger<DatabaseContext> logger
) : PostgreSqlDbContext(connectionStringProvider, appSettingsDatabaseOptions, logger)
{
    public DbSet<InteractivityStorageElement> Interactivity { get; set; }
    public DbSet<ReleaseVersionStorageElement> Releases { get; set; }
    public DbSet<RoleStorageElement> Roles { get; set; }
    public DbSet<LockStorageElement> Locks { get; set; }
}