using AntiClown.Data.Api.Core.Database;
using Microsoft.EntityFrameworkCore.Design;

namespace AntiClown.Data.Api.PostgreSqlMigrationsApplier;

public class DevContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        return new DatabaseContext("Host=localhost;Port=5432;Database=AntiClownDataApi;Username=postgres;Password=postgres;Include Error Detail=true");
    }
}