using AntiClown.DiscordBot.Database;
using Microsoft.EntityFrameworkCore.Design;

namespace AntiClown.DiscordBot.PostgreSqlMigrationsApplier;

public class DevContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        return new DatabaseContext("Host=localhost;Port=5432;Database=AntiClownDiscordBot;Username=postgres;Password=postgres;Include Error Detail=true");
    }
}