using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace AntiClown.Tests.Configuration;

public abstract class IntegrationTestsBase<TFactory, TEntryPoint>
    where TFactory : IntegrationTestsWebApplicationFactory<TEntryPoint>, new()
    where TEntryPoint : class
{
    protected TFactory Factory { get; private set; } = null!;
    protected IServiceScope Scope { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task InitializeFactory()
    {
        Factory = new TFactory();
        await Factory.InitializeAsync();
        Scope = Factory.Services.CreateScope();
    }

    [OneTimeTearDown]
    public async Task DisposeFactory()
    {
        Scope?.Dispose();
        await Factory.DisposeAsync();
    }
}
