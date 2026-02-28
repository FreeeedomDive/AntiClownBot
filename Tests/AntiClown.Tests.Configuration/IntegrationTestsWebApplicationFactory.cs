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

            RemoveMassTransitHostedService(services);
            MockIBus(services);

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

    private static void RemoveMassTransitHostedService(IServiceCollection services)
    {
        var massTransitHostedServices = services
            .Where(d => d.ServiceType == typeof(IHostedService)
                        && (d.ImplementationType?.Namespace?.StartsWith("MassTransit") ?? false))
            .ToList();
        massTransitHostedServices.ForEach(s => services.Remove(s));
    }

    private static void MockIBus(IServiceCollection services)
    {
        var mockBus = Substitute.For<IBus>();
        services.RemoveAll<IBus>();
        services.RemoveAll<IPublishEndpoint>();
        services.RemoveAll<ISendEndpointProvider>();
        services.AddSingleton(mockBus);
        services.AddSingleton<IPublishEndpoint>(mockBus);
        services.AddSingleton<ISendEndpointProvider>(mockBus);
    }
}
