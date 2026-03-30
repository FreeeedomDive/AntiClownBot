using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Races;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Teams;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;
using FluentAssertions;
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
        jolpicaClient = Substitute.For<IJolpicaClient>();
        scheduler = Substitute.For<IScheduler>();
        timeProvider = Substitute.For<TimeProvider>();

        service = new F1PredictionsService(
            racesRepository,
            resultsRepository,
            messageProducer,
            teamsRepository,
            resultBuilder,
            jolpicaClient,
            scheduler,
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
    public async Task PollQualifyingGridAsync_Should_ScheduleRepoll_WhenJolpicaReturnsNull()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero));
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(null));

        await service.PollQualifyingGridAsync(testRaceId);

        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_ScheduleRepoll_WhenJolpicaReturnsEmpty()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>([]));

        await service.PollQualifyingGridAsync(testRaceId);

        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotScheduleRepoll_WhenJolpicaReturnsData()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(["DriverA1", "DriverA2", "DriverB1", "DriverB2"]));

        await service.PollQualifyingGridAsync(testRaceId);

        scheduler.DidNotReceive().Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotCallUpdate_WhenJolpicaReturnsNull()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(null));

        await service.PollQualifyingGridAsync(testRaceId);

        await racesRepository.DidNotReceive().UpdateAsync(Arg.Any<F1Race>());
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_SaveJolpicaDriversInOrder_WhenAllDriversPresent()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var jolpicaOrder = new[] { "DriverB2", "DriverA1", "DriverA2", "DriverB1" };
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(jolpicaOrder));

        await service.PollQualifyingGridAsync(testRaceId);

        await racesRepository.Received(1).UpdateAsync(
            Arg.Is<F1Race>(r =>
                r.QualifyingGrid != null &&
                r.QualifyingGrid.Take(4).SequenceEqual(jolpicaOrder)
            )
        );
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_AppendMissingDrivers_WhenJolpicaGridIsPartial()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero));
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(["DriverA1", "DriverB1"]));

        await service.PollQualifyingGridAsync(testRaceId);

        await racesRepository.Received(1).UpdateAsync(
            Arg.Is<F1Race>(r =>
                r.QualifyingGrid != null &&
                r.QualifyingGrid[0] == "DriverA1" &&
                r.QualifyingGrid[1] == "DriverB1" &&
                r.QualifyingGrid.Contains("DriverA2") &&
                r.QualifyingGrid.Contains("DriverB2") &&
                r.QualifyingGrid.Length == 4
            )
        );
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_UseRoundIndex_1_WhenOnlyOneNonSprintRaceInSeason()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2028, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var capturedIndex = -1;
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
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
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
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
    public async Task PollQualifyingGridAsync_Should_CallJolpicaWithRaceSeason()
    {
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var capturedSeason = -1;
        jolpicaClient
            .GetQualifyingDriverNamesAsync(Arg.Is<int>(_ => true), Arg.Any<int>())
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
    private IF1PredictionsMessageProducer messageProducer = null!;

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