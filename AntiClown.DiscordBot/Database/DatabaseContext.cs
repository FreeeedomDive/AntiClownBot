using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Releases.Repositories;
using AntiClown.DiscordBot.Roles.Repositories;
using AntiClown.DiscordBot.Utility.Locks;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.ContextBuilders;

namespace AntiClown.DiscordBot.Database;

public class DatabaseContext(string connectionString) : PostgreSqlDbContext(connectionString)
{
    public DbSet<InteractivityStorageElement> Interactivity { get; set; }
    public DbSet<ReleaseVersionStorageElement> Releases { get; set; }
    public DbSet<RoleStorageElement> Roles { get; set; }
    public DbSet<LockStorageElement> Locks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString, builder => builder.MigrationsAssembly("AntiClown.DiscordBot.PostgreSqlMigrationsApplier"));
    }
}