using AntiClown.Api.Core.Economies.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace AntiClown.Api.Core.IntegrationTests.Economies;

public class LohotronTestsWebApplicationFactory : ApiIntegrationTestsWebApplicationFactory
{
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);
        services.RemoveAll<ILohotronRewardGenerator>();
        services.AddSingleton(_ => Substitute.For<ILohotronRewardGenerator>());
    }
}
