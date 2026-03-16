using AntiClown.Entertainment.Api.Core.Database;
using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;
using AntiClown.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests;

public class EntertainmentApiIntegrationTestsWebApplicationFactory
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
        services.RemoveAll<IJolpicaClient>();
        services.AddSingleton(_ => Substitute.For<IJolpicaClient>());
        services.RemoveAll<TimeProvider>();
        services.AddSingleton(_ => Substitute.For<TimeProvider>());
    }
}