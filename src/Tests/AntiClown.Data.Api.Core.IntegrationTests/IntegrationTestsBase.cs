using AntiClown.Data.Api.Core.Rights.Services;
using AntiClown.Data.Api.Core.SettingsStoring.Services;
using AntiClown.Data.Api.Core.Tokens.Services;
using AntiClown.Tests.Configuration;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;

namespace AntiClown.Data.Api.Core.IntegrationTests;

public abstract class IntegrationTestsBase
    : IntegrationTestsBase<DataApiIntegrationTestsWebApplicationFactory, Program>
{
    protected ISettingsService SettingsService { get; private set; } = null!;
    protected ITokensService TokensService { get; private set; } = null!;
    protected IRightsService RightsService { get; private set; } = null!;
    protected IFixture Fixture { get; private set; } = null!;

    [OneTimeSetUp]
    public void InitializeServices()
    {
        Fixture = new Fixture();
        SettingsService = Scope.ServiceProvider.GetRequiredService<ISettingsService>();
        TokensService = Scope.ServiceProvider.GetRequiredService<ITokensService>();
        RightsService = Scope.ServiceProvider.GetRequiredService<IRightsService>();
    }
}
