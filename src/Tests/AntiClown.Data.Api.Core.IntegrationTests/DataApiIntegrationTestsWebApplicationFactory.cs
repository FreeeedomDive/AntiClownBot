using AntiClown.Data.Api.Core.Database;
using AntiClown.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AntiClown.Data.Api.Core.IntegrationTests;

public class DataApiIntegrationTestsWebApplicationFactory
    : IntegrationTestsWebApplicationFactory<Program>
{
    protected override async Task RunMigrationsAsync(IServiceScope scope)
    {
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DatabaseContext>>();
        await using var db = await dbContextFactory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
    }
}
