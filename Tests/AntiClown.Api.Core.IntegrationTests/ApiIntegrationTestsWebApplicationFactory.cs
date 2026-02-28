using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.IntegrationTests.Mocks;
using AntiClown.Core.Schedules;
using AntiClown.Data.Api.Client;
using AntiClown.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NSubstitute;

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
        RemoveHangfireHostedServices(services);
        MockScheduler(services);
        MockAntiClownDataApiClient(services);
    }

    private static void RemoveHangfireHostedServices(IServiceCollection services)
    {
        var hangfireHosted = services
            .Where(d => d.ServiceType == typeof(IHostedService)
                        && (d.ImplementationType?.Namespace?.StartsWith("Hangfire") ?? false))
            .ToList();
        hangfireHosted.ForEach(s => services.Remove(s));
    }

    private static void MockScheduler(IServiceCollection services)
    {
        services.RemoveAll<IScheduler>();
        services.AddTransient<IScheduler, SchedulerMock>();
    }

    private static void MockAntiClownDataApiClient(IServiceCollection services)
    {
        services.RemoveAll<IAntiClownDataApiClient>();
        services.AddSingleton(_ => Substitute.For<IAntiClownDataApiClient>());
    }
}
