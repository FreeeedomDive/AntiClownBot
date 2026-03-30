using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Statistics;
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
        F1PredictionsService = Scope.ServiceProvider.GetRequiredService<IF1PredictionsService>();
        F1PredictionsStatisticsService = Scope.ServiceProvider.GetRequiredService<IF1PredictionsStatisticsService>();
        F1BingoBoardsService = Scope.ServiceProvider.GetRequiredService<IF1BingoBoardsService>();
        F1BingoCardsService = Scope.ServiceProvider.GetRequiredService<IF1BingoCardsService>();
        F1ChampionshipPredictionsService = Scope.ServiceProvider.GetRequiredService<IF1ChampionshipPredictionsService>();
        PartiesService = Scope.ServiceProvider.GetRequiredService<IPartiesService>();
        JolpicaClientMock = Scope.ServiceProvider.GetRequiredService<IJolpicaClient>();
        TimeProviderMock = Scope.ServiceProvider.GetRequiredService<TimeProvider>();
    }

    protected IF1PredictionsService F1PredictionsService { get; private set; } = null!;
    protected IF1PredictionsStatisticsService F1PredictionsStatisticsService { get; private set; } = null!;
    protected IF1BingoBoardsService F1BingoBoardsService { get; private set; } = null!;
    protected IF1BingoCardsService F1BingoCardsService { get; private set; } = null!;
    protected IF1ChampionshipPredictionsService F1ChampionshipPredictionsService { get; private set; } = null!;
    protected IPartiesService PartiesService { get; private set; } = null!;
    protected IJolpicaClient JolpicaClientMock { get; private set; } = null!;
    protected TimeProvider TimeProviderMock { get; private set; } = null!;
    protected IFixture Fixture { get; private set; } = null!;
}