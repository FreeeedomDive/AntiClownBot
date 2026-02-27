using AntiClown.Data.Api.Core.Rights.Repositories;
using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AntiClown.Data.Api.Core.Tokens.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.ContextBuilders;

namespace AntiClown.Data.Api.Core.Database;

public class DatabaseContext(string connectionString) : PostgreSqlDbContext(connectionString)
{
    public DbSet<SettingStorageElement> Settings { get; set; }
    public DbSet<TokenStorageElement> Tokens { get; set; }
    public DbSet<RightsStorageElement> Rights { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString, builder => builder.MigrationsAssembly("AntiClown.Data.Api.PostgreSqlMigrationsApplier"));
    }
}