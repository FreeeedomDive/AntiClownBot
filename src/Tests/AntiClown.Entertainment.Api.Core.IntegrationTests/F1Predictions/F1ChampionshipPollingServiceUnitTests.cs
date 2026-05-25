using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;
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

public class F1ChampionshipPollingServiceUnitTests
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
        scheduler = Substitute.For<IScheduler>();
        timeProvider = Substitute.For<TimeProvider>();

        var pollingOptions = Microsoft.Extensions.Options.Options.Create(new F1PredictionsOptions
        {
            ChampionshipPollingInterval = TimeSpan.FromMinutes(10),
            QualifyingGridPollingInterval = TimeSpan.FromMinutes(30),
        });

        service = new F1PredictionsService(
            racesRepository,
            resultsRepository,
            messageProducer,
            teamsRepository,
            resultBuilder,
            championshipPredictionsService,
            jolpicaClient,
            scheduler,
            pollingOptions,
            NullLogger<F1PredictionsService>.Instance,
            timeProvider
        );

        testRaceId = Guid.NewGuid();
        testRace = CreateFinishedNonSprintRace(testRaceId, season: 2026);

        racesRepository.ReadAsync(testRaceId).Returns(testRace);

        championshipPredictionsService
            .ReadResultsAsync(Arg.Any<int>())
            .Returns(new F1ChampionshipResults { HasData = false, Standings = [] });
        championshipPredictionsService
            .WriteResultsAsync(Arg.Any<int>(), Arg.Any<F1ChampionshipResults>())
            .Returns(Task.CompletedTask);
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_ScheduleRepoll_WhenJolpicaReturnsNull()
    {
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([testRace]);
        jolpicaClient.GetDriverStandingsAsync(Arg.Any<int>()).Returns(Task.FromResult<(int, string[])?>(null));

        await service.PollChampionshipResultsAsync(testRaceId);

        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_ScheduleRepoll_WhenJolpicaRoundIsOlderThanExpected()
    {
        var finishedRace1 = CreateFinishedNonSprintRace(Guid.NewGuid(), season: 2026);
        var finishedRace2 = CreateFinishedNonSprintRace(Guid.NewGuid(), season: 2026);
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([finishedRace1, finishedRace2, testRace]);

        // Jolpica returns round 2, but we already finished 3 non-sprint races
        jolpicaClient.GetDriverStandingsAsync(2026).Returns(Task.FromResult<(int, string[])?>(
            (2, ["Verstappen", "Hamilton"])
        ));

        await service.PollChampionshipResultsAsync(testRaceId);

        scheduler.Received(1).Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_NotScheduleRepoll_WhenJolpicaRoundMatchesExpected()
    {
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([testRace]);

        jolpicaClient.GetDriverStandingsAsync(2026).Returns(Task.FromResult<(int, string[])?>(
            (1, ["Verstappen", "Hamilton"])
        ));

        await service.PollChampionshipResultsAsync(testRaceId);

        scheduler.DidNotReceive().Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_NotScheduleRepoll_WhenJolpicaRoundIsAheadOfExpected()
    {
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([testRace]);

        // Jolpica returns round 2 but only 1 race finished — still valid (more data than expected is ok)
        jolpicaClient.GetDriverStandingsAsync(2026).Returns(Task.FromResult<(int, string[])?>(
            (2, ["Hamilton", "Verstappen"])
        ));

        await service.PollChampionshipResultsAsync(testRaceId);

        scheduler.DidNotReceive().Schedule(Arg.Any<Action>());
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_WriteStandings_WhenDataIsFresh()
    {
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([testRace]);
        var expectedStandings = new[] { "Verstappen", "Hamilton", "Leclerc" };
        jolpicaClient.GetDriverStandingsAsync(2026).Returns(Task.FromResult<(int, string[])?>(
            (1, expectedStandings)
        ));

        await service.PollChampionshipResultsAsync(testRaceId);

        await championshipPredictionsService.Received(1).WriteResultsAsync(
            2026,
            Arg.Is<F1ChampionshipResults>(r => r.Standings.SequenceEqual(expectedStandings))
        );
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_NotWriteStandings_WhenDataIsStale()
    {
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([testRace, CreateFinishedNonSprintRace(Guid.NewGuid(), 2026)]);
        jolpicaClient.GetDriverStandingsAsync(2026).Returns(Task.FromResult<(int, string[])?>(
            (1, ["Verstappen"])
        ));

        await service.PollChampionshipResultsAsync(testRaceId);

        await championshipPredictionsService.DidNotReceive().WriteResultsAsync(Arg.Any<int>(), Arg.Any<F1ChampionshipResults>());
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_Skip_WhenRaceIsSprint()
    {
        var sprintRaceId = Guid.NewGuid();
        var sprintRace = CreateFinishedSprintRace(sprintRaceId, season: 2026);
        racesRepository.ReadAsync(sprintRaceId).Returns(sprintRace);

        await service.PollChampionshipResultsAsync(sprintRaceId);

        scheduler.DidNotReceive().Schedule(Arg.Any<Action>());
        await jolpicaClient.DidNotReceive().GetDriverStandingsAsync(Arg.Any<int>());
        await championshipPredictionsService.DidNotReceive().WriteResultsAsync(Arg.Any<int>(), Arg.Any<F1ChampionshipResults>());
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_ExcludeSprintRaces_WhenCalculatingExpectedRound()
    {
        var sprintRace = CreateFinishedSprintRace(Guid.NewGuid(), season: 2026);
        // 1 non-sprint (testRace) + 1 sprint = expectedRound should be 1
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([testRace, sprintRace]);

        var capturedSeason = -1;
        jolpicaClient.GetDriverStandingsAsync(Arg.Any<int>())
            .Returns(callInfo =>
                {
                    capturedSeason = callInfo.ArgAt<int>(0);
                    return Task.FromResult<(int, string[])?>(null);
                }
            );

        await service.PollChampionshipResultsAsync(testRaceId);

        // expectedRound = 1 (only testRace is non-sprint), Jolpica returned null → reschedule
        scheduler.Received(1).Schedule(Arg.Any<Action>());
        capturedSeason.Should().Be(2026);
    }

    [Test]
    public async Task PollChampionshipResultsAsync_Should_QueryJolpicaWithRaceSeason()
    {
        var race2040Id = Guid.NewGuid();
        var race2040 = CreateFinishedNonSprintRace(race2040Id, season: 2040);
        racesRepository.ReadAsync(race2040Id).Returns(race2040);
        racesRepository.FindAsync(Arg.Any<F1RaceFilter>()).Returns([race2040]);

        var capturedSeason = -1;
        jolpicaClient.GetDriverStandingsAsync(Arg.Any<int>())
            .Returns(callInfo =>
                {
                    capturedSeason = callInfo.ArgAt<int>(0);
                    return Task.FromResult<(int, string[])?>(null);
                }
            );

        await service.PollChampionshipResultsAsync(race2040Id);

        capturedSeason.Should().Be(2040);
    }

    private static F1Race CreateFinishedNonSprintRace(Guid id, int season) =>
        CreateRace(id, season, isSprint: false);

    private static F1Race CreateFinishedSprintRace(Guid id, int season) =>
        CreateRace(id, season, isSprint: true);

    private static F1Race CreateRace(Guid id, int season, bool isSprint) => new()
    {
        Id = id,
        Season = season,
        Name = "Test GP",
        IsSprint = isSprint,
        IsActive = false,
        IsOpened = false,
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

    private IJolpicaClient jolpicaClient = null!;
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
