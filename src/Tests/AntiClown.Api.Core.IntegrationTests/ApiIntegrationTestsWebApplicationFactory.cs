using AntiClown.Api.Core.Database;
using AntiClown.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AntiClown.Api.Core.IntegrationTests;

public class ApiIntegrationTestsWebApplicationFactory
    : IntegrationTestsWebApplicationFactory<Program>
{
    protected override async Task RunMigrationsAsync(IServiceScope scope)
    {
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DatabaseContext>>();
        await using var db = await dbContextFactory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
    }

    protected override void ConfigureTestServices(IServiceCollection services)
    {
    }
}
