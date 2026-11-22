using AntiClown.Entertainment.Api.Core.Parties.Services;
using AntiClown.Tests.Configuration;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests;

public abstract class IntegrationTestsBase
    : IntegrationTestsBase<EntertainmentApiIntegrationTestsWebApplicationFactory, Program>
{
    [OneTimeSetUp]
    public void InitializeServices()
    {
        Fixture = new Fixture();
        PartiesService = Scope.ServiceProvider.GetRequiredService<IPartiesService>();
    }

    protected IPartiesService PartiesService { get; private set; } = null!;
    protected IFixture Fixture { get; private set; } = null!;
}
