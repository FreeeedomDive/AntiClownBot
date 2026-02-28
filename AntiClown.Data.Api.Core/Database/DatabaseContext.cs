using AntiClown.Data.Api.Core.Rights.Repositories;
using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AntiClown.Data.Api.Core.Tokens.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Core.Contexts;
using SqlRepositoryBase.Core.Options;

namespace AntiClown.Data.Api.Core.Database;

public class DatabaseContext(
    IConnectionStringProvider connectionStringProvider,
    IOptions<AppSettingsDatabaseOptions> appSettingsDatabaseOptions,
    ILogger<DatabaseContext> logger
) : PostgreSqlDbContext(connectionStringProvider, appSettingsDatabaseOptions, logger)
{
    public DbSet<SettingStorageElement> Settings { get; set; }
    public DbSet<TokenStorageElement> Tokens { get; set; }
    public DbSet<RightsStorageElement> Rights { get; set; }
}