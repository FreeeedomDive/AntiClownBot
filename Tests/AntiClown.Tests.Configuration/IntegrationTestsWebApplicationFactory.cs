using AntiClown.Api.Client;
using AntiClown.Core.Schedules;
using AntiClown.Data.Api.Client;
using AntiClown.Tests.Configuration.Mocks;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using SqlRepositoryBase.Core.Options;
using Testcontainers.PostgreSql;

namespace AntiClown.Tests.Configuration;

public abstract class IntegrationTestsWebApplicationFactory<TEntryPoint>
    : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    protected abstract Task RunMigrationsAsync(IServiceScope scope);

    protected virtual void ConfigureTestServices(IServiceCollection services) { }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        container.StartAsync().GetAwaiter().GetResult();
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IConnectionStringProvider>();
            services.AddSingleton<IConnectionStringProvider>(_ =>
                new TestContainerConnectionStringProvider(container));

            MockMassTransit(services);
            RemoveHangfireHostedServices(services);
            MockScheduler(services);
            MockAntiClownApiClient(services);
            MockAntiClownDataApiClient(services);
            ConfigureTestServices(services);
        });
    }

    public async Task InitializeAsync()
    {
        _ = Services;
        using var scope = Services.CreateScope();
        await RunMigrationsAsync(scope);
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await container.DisposeAsync();
    }

    protected static void RemoveHangfireHostedServices(IServiceCollection services)
    {
        var hangfireHosted = services
            .Where(d => d.ServiceType == typeof(IHostedService)
                        && (d.ImplementationType?.Namespace?.StartsWith("Hangfire") ?? false))
            .ToList();
        hangfireHosted.ForEach(s => services.Remove(s));
    }

    protected static void MockScheduler(IServiceCollection services)
    {
        services.RemoveAll<IScheduler>();
        services.AddTransient<IScheduler, SchedulerMock>();
    }

    protected static void MockAntiClownApiClient(IServiceCollection services)
    {
        services.RemoveAll<IAntiClownApiClient>();
        services.AddSingleton(_ => Substitute.For<IAntiClownApiClient>());
    }

    protected static void MockAntiClownDataApiClient(IServiceCollection services)
    {
        services.RemoveAll<IAntiClownDataApiClient>();
        services.AddSingleton(_ => Substitute.For<IAntiClownDataApiClient>());
    }

    private static void MockMassTransit(IServiceCollection services)
    {
        var massTransitHostedServices = services
            .Where(d => d.ServiceType == typeof(IHostedService)
                        && (d.ImplementationType?.Namespace?.StartsWith("MassTransit") ?? false))
            .ToList();
        massTransitHostedServices.ForEach(s => services.Remove(s));
        var mockBus = Substitute.For<IBus>();
        services.RemoveAll<IBus>();
        services.RemoveAll<IPublishEndpoint>();
        services.RemoveAll<ISendEndpointProvider>();
        services.AddSingleton(mockBus);
        services.AddSingleton<IPublishEndpoint>(mockBus);
        services.AddSingleton<ISendEndpointProvider>(mockBus);
    }
}
