using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AntiClown.Data.Api.Core.Tokens.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.ContextBuilders;

namespace AntiClown.Data.Api.Core.Database;

public class DatabaseContext : PostgreSqlDbContext
{
    public DatabaseContext(string connectionString) : base(connectionString)
    {
    }

    public DbSet<SettingStorageElement> Settings { get; set; }
    public DbSet<TokenStorageElement> Tokens { get; set; }
}