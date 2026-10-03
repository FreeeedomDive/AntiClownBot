using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
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
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1QualifyingGridServiceUnitTests
{
    [SetUp]
    public void SetUp()
    {
        racesRepository = Substitute.For<IF1RacesRepository>();
        resultsRepository = Substitute.For<IF1PredictionResultsRepository>();
        messageProducer = Substitute.For<IF1PredictionsMessageProducer>();
        teamsRepository = Substitute.For<IF1PredictionTeamsRepository>();
        resultBuilder = Substitute.For<IF1PredictionsResultBuilder>();
        championshipPredictionsService = Substitute.For<IF1ChampionshipPredictionsService>();
        jolpicaClient = Substitute.For<IJolpicaClient>();
        startingGridClient = Substitute.For<IStartingGridClient>();
        scheduler = Substitute.For<IScheduler>();
        timeProvider = Substitute.For<TimeProvider>();

        service = new F1PredictionsService(
            racesRepository,
            resultsRepository,
            messageProducer,
            teamsRepository,
            resultBuilder,
            championshipPredictionsService,
            jolpicaClient,
            startingGridClient,
            scheduler,
            Microsoft.Extensions.Options.Options.Create(new F1PredictionsOptions()),
            NullLogger<F1PredictionsService>.Instance,
            timeProvider
        );

        testRaceId = Guid.NewGuid();
        testRace = CreateRace(testRaceId);

        racesRepository.ReadAsync(testRaceId).Returns(testRace);
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([testRace]);
        racesRepository.UpdateAsync(Arg.Any<F1Race>()).Returns(Task.CompletedTask);

        teamsRepository.ReadAllAsync().Returns(
            [
                new F1Team("TeamA", "DriverA1", "DriverA2"),
                new F1Team("TeamB", "DriverB1", "DriverB2"),
            ]
        );
    }

    [Test]
    public async Task SaveQualifyingGridAsync_Should_PersistProvidedGrid()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var grid = new[] { "DriverB1", "DriverA1", "DriverA2", "DriverB2" };

        await service.SaveQualifyingGridAsync(testRaceId, grid);

        await racesRepository.Received(1).UpdateAsync(
            Arg.Is<F1Race>(r =>
                r.QualifyingGrid != null &&
                r.QualifyingGrid.SequenceEqual(grid)
            )
        );
        await messageProducer.Received(1).ProduceStartingGridUpdatedAsync(testRaceId);
    }

    [Test]
    public async Task SaveQualifyingGridAsync_Should_OverwriteExistingGrid()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero));
        testRace.QualifyingGrid = ["DriverA1", "DriverA2"];
        var newGrid = new[] { "DriverB1", "DriverB2" };

        await service.SaveQualifyingGridAsync(testRaceId, newGrid);

        await racesRepository.Received(1).UpdateAsync(
            Arg.Is<F1Race>(r =>
                r.QualifyingGrid != null &&
                r.QualifyingGrid.SequenceEqual(newGrid)
            )
        );
    }

    [Test]
    public async Task SaveQualifyingGridAsync_Should_NotPublishUnchangedGrid()
    {
        testRace.QualifyingGrid = ["DriverA1", "DriverA2"];

        await service.SaveQualifyingGridAsync(testRaceId, ["DriverA1", "DriverA2"]);

        await racesRepository.DidNotReceive().UpdateAsync(Arg.Any<F1Race>());
        await messageProducer.DidNotReceive().ProduceStartingGridUpdatedAsync(Arg.Any<Guid>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_ScheduleRepoll_WhenOpenF1ReturnsNull()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero));
        startingGridClient
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(null));

        await service.PollQualifyingGridAsync(testRaceId);

        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_ScheduleRepoll_WhenOpenF1ReturnsEmpty()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));
        startingGridClient
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>([]));

        await service.PollQualifyingGridAsync(testRaceId);

        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_ScheduleRepoll_WhenGridIsAvailable()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        startingGridClient
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(["DriverA1", "DriverA2", "DriverB1", "DriverB2"]));

        await service.PollQualifyingGridAsync(testRaceId);

        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_StopAfterRaceFinishes()
    {
        testRace.IsActive = false;

        await service.PollQualifyingGridAsync(testRaceId);

        await startingGridClient.DidNotReceive().GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        scheduler.DidNotReceive().Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotUpdateOrPublishWhenGridUnchanged()
    {
        testRace.QualifyingGrid = ["DriverA1", "DriverA2", "DriverB1", "DriverB2"];
        startingGridClient.GetDriverNamesAsync(2026, 1, false).Returns(["DriverA1", "DriverA2", "DriverB1", "DriverB2"]);

        await service.PollQualifyingGridAsync(testRaceId);

        await racesRepository.DidNotReceive().UpdateAsync(Arg.Any<F1Race>());
        await messageProducer.DidNotReceive().ProduceStartingGridUpdatedAsync(Arg.Any<Guid>());
        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_PublishWhenPenaltyChangesGrid()
    {
        testRace.QualifyingGrid = ["DriverA1", "DriverA2", "DriverB1", "DriverB2"];
        startingGridClient.GetDriverNamesAsync(2026, 1, false).Returns(["DriverA2", "DriverB1", "DriverA1", "DriverB2"]);

        await service.PollQualifyingGridAsync(testRaceId);

        testRace.QualifyingGrid.Should().Equal("DriverA2", "DriverB1", "DriverA1", "DriverB2");
        await messageProducer.Received(1).ProduceStartingGridUpdatedAsync(testRaceId);
        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_RetryAfterClientException()
    {
        startingGridClient.GetDriverNamesAsync(2026, 1, false)
            .Returns<Task<string[]?>>(_ => throw new HttpRequestException("OpenF1 unavailable"));

        await service.PollQualifyingGridAsync(testRaceId);

        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_RequestSprintGridForSprintRace()
    {
        testRace.IsSprint = true;

        await service.PollQualifyingGridAsync(testRaceId);

        await startingGridClient.Received(1).GetDriverNamesAsync(2026, 1, true);
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotCallUpdate_WhenOpenF1ReturnsNull()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        startingGridClient
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(null));

        await service.PollQualifyingGridAsync(testRaceId);

        await racesRepository.DidNotReceive().UpdateAsync(Arg.Any<F1Race>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_SaveGridDriversInOrder_WhenAllDriversPresent()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var gridOrder = new[] { "DriverB2", "DriverA1", "DriverA2", "DriverB1" };
        startingGridClient
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(gridOrder));

        await service.PollQualifyingGridAsync(testRaceId);

        await racesRepository.Received(1).UpdateAsync(
            Arg.Is<F1Race>(r =>
                r.QualifyingGrid != null &&
                r.QualifyingGrid.Take(4).SequenceEqual(gridOrder)
            )
        );
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotFabricateMissingDrivers()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero));
        startingGridClient
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(["DriverA1", "DriverB1"]));

        await service.PollQualifyingGridAsync(testRaceId);

        await racesRepository.Received(1).UpdateAsync(
            Arg.Is<F1Race>(r =>
                r.QualifyingGrid != null &&
                r.QualifyingGrid[0] == "DriverA1" &&
                r.QualifyingGrid[1] == "DriverB1" &&
                r.QualifyingGrid.Length == 2
            )
        );
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_UseRoundIndex_1_WhenOnlyOneNonSprintRaceInSeason()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2028, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var capturedIndex = -1;
        startingGridClient
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(callInfo =>
                {
                    capturedIndex = callInfo.ArgAt<int>(1);
                    return Task.FromResult<string[]?>(null);
                }
            );

        await service.PollQualifyingGridAsync(testRaceId);

        capturedIndex.Should().Be(1);
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_ExcludeSprintRaces_InRoundIndexCalculation()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2029, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var nonSprintFirst = CreateRace(Guid.NewGuid(), isSprint: false);
        var sprintRace = CreateRace(Guid.NewGuid(), isSprint: true);

        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns(
            Task.FromResult(new[] { nonSprintFirst, sprintRace, testRace })
        );

        var capturedIndex = -1;
        startingGridClient
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(callInfo =>
                {
                    capturedIndex = callInfo.ArgAt<int>(1);
                    return Task.FromResult<string[]?>(null);
                }
            );

        await service.PollQualifyingGridAsync(testRaceId);

        capturedIndex.Should().Be(2);
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_CallOpenF1WithRaceSeason()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var capturedSeason = -1;
        startingGridClient
            .GetDriverNamesAsync(Arg.Is<int>(_ => true), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(callInfo =>
                {
                    capturedSeason = callInfo.ArgAt<int>(0);
                    return Task.FromResult<string[]?>(null);
                }
            );

        await service.PollQualifyingGridAsync(testRaceId);

        capturedSeason.Should().Be(testRace.Season);
    }

    private static F1Race CreateRace(Guid id, int season = 2026, bool isSprint = false)
    {
        return new F1Race
        {
            Id = id,
            Season = season,
            Name = "Test GP",
            IsSprint = isSprint,
            IsActive = true,
            IsOpened = true,
            Predictions = [],
            Result = new F1PredictionRaceResult
            {
                RaceId = id,
                Classification = [],
                DnfDrivers = [],
                SafetyCars = 0,
                FirstPlaceLead = 0,
            },
        };
    }

    private IJolpicaClient jolpicaClient = null!;
    private IStartingGridClient startingGridClient = null!;
    private IF1PredictionsMessageProducer messageProducer = null!;
    private IF1ChampionshipPredictionsService championshipPredictionsService = null!;
    private IF1RacesRepository racesRepository = null!;
    private IF1PredictionsResultBuilder resultBuilder = null!;
    private IF1PredictionResultsRepository resultsRepository = null!;
    private IScheduler scheduler = null!;
    private IF1PredictionsService service = null!;
    private IF1PredictionTeamsRepository teamsRepository = null!;
    private TimeProvider timeProvider = null!;
    private F1Race testRace = null!;
    private Guid testRaceId;
}
