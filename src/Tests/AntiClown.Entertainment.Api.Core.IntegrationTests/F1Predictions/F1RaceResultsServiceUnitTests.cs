using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;
using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.OpenF1;
using AntiClown.Entertainment.Api.Core.F1Predictions.Options;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Races;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Teams;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1RaceResultsServiceUnitTests
{
    [SetUp]
    public void SetUp()
    {
        racesRepository = Substitute.For<IF1RacesRepository>();
        resultsRepository = Substitute.For<IF1PredictionResultsRepository>();
        messageProducer = Substitute.For<IF1PredictionsMessageProducer>();
        resultBuilder = Substitute.For<IF1PredictionsResultBuilder>();
        service = new F1PredictionsService(
            racesRepository,
            resultsRepository,
            messageProducer,
            Substitute.For<IF1PredictionTeamsRepository>(),
            resultBuilder,
            Substitute.For<IF1ChampionshipPredictionsService>(),
            Substitute.For<IJolpicaClient>(),
            Substitute.For<IStartingGridClient>(),
            Substitute.For<IScheduler>(),
            Microsoft.Extensions.Options.Options.Create(new F1PredictionsOptions()),
            NullLogger<F1PredictionsService>.Instance,
            Substitute.For<TimeProvider>()
        );

        raceId = Guid.NewGuid();
        race = new F1Race { Id = raceId, IsActive = true };
        racesRepository.ReadAsync(raceId).Returns(race);
        resultBuilder.Build(race).Returns([]);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task AddRaceResultAsync_PublishesOnlyForActiveRace(bool isActive)
    {
        race.IsActive = isActive;
        var result = new F1PredictionRaceResult { RaceId = raceId, Classification = [], DnfDrivers = [] };

        await service.AddRaceResultAsync(raceId, result);

        await racesRepository.Received(1).UpdateAsync(Arg.Is<F1Race>(r => r.Result == result));
        if (isActive)
        {
            await messageProducer.Received(1).ProduceRaceResultUpdatedAsync(raceId);
        }
        else
        {
            await messageProducer.DidNotReceive().ProduceRaceResultUpdatedAsync(raceId);
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task FinishRaceAsync_PublishesOnlyOnFirstFinish(bool isActive)
    {
        race.IsActive = isActive;

        await service.FinishRaceAsync(raceId);

        await resultsRepository.Received(1).CreateOrUpdateAsync(Arg.Any<F1PredictionResult[]>());
        await racesRepository.Received(1).UpdateAsync(Arg.Is<F1Race>(r => !r.IsActive));
        if (isActive)
        {
            await messageProducer.Received(1).ProduceRaceFinishedAsync(raceId);
        }
        else
        {
            await messageProducer.DidNotReceive().ProduceRaceFinishedAsync(raceId);
        }
    }

    private IF1RacesRepository racesRepository = null!;
    private IF1PredictionResultsRepository resultsRepository = null!;
    private IF1PredictionsMessageProducer messageProducer = null!;
    private IF1PredictionsResultBuilder resultBuilder = null!;
    private F1PredictionsService service = null!;
    private Guid raceId;
    private F1Race race = null!;
}
