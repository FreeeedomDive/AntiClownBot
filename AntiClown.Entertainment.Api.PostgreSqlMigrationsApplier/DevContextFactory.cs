using AntiClown.Entertainment.Api.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier;

public class DevContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        return new DatabaseContext("Host=localhost;Port=5432;Database=AntiClownEntertainmentApi;Username=postgres;Password=postgres;Include Error Detail=true");
    }
}