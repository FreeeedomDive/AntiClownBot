using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Core.Extensions;

namespace AntiClown.Api.Core.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(
        DbContextOptions<DatabaseContext> options,
        IOptions<DatabaseOptions> dbOptionsAccessor
    ) : base(options)
    {
        Options = dbOptionsAccessor.Value;
    }

    public DatabaseContext()
    {
        var connectionString = Environment.GetEnvironmentVariable("AntiClown.Tests.PostgreSqlConnectionString")
                               ?? throw new InvalidOperationException("No ConnectionString was provided");
        Options = new DatabaseOptions
        {
            ConnectionString = connectionString
        };
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.CreateVersionForTable<EconomyStorageElement>(nameof(Economies));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Options.ConnectionString);
    }

    public DbSet<UserStorageElement> Users { get; set; }
    public DbSet<EconomyStorageElement> Economies { get; set; }
    private DatabaseOptions Options { get; }
}