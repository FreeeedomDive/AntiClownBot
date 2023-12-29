using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Releases.Repositories;
using AntiClown.DiscordBot.Roles.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.ContextBuilders;

namespace AntiClown.DiscordBot.Database;

public class DatabaseContext : PostgreSqlDbContext
{
    public DatabaseContext(string connectionString) : base(connectionString)
    {
    }

    public DbSet<InteractivityStorageElement> Interactivity { get; set; }
    public DbSet<ReleaseVersionStorageElement> Releases { get; set; }
    public DbSet<RoleStorageElement> Roles { get; set; }
}