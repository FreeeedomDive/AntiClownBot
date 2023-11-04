using AntiClown.Data.Api.Core.Options;
using AntiClown.Data.Api.Core.SettingsStoring.Repositories;
using AntiClown.Data.Api.Core.Tokens.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AntiClown.Data.Api.Core.Database;

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

    public DbSet<SettingStorageElement> Settings { get; set; }
    public DbSet<TokenStorageElement> Tokens { get; set; }

    private DatabaseOptions Options { get; }
}