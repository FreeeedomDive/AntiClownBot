using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;
using FluentAssertions;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1PredictionsServiceTests : IntegrationTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        testTeam = new F1Team("TestTeamDriver1", "Driver1", "Driver2");
        await F1PredictionsService.CreateOrUpdateTeamAsync(testTeam);
    }

    [Test]
    public async Task CreateOrUpdateTeamAsync_Should_CreateTeam()
    {
        var teams = await F1PredictionsService.GetActiveTeamsAsync();

        teams.Should().Contain(t =>
            t.Name == testTeam.Name
            && t.FirstDriver == testTeam.FirstDriver
            && t.SecondDriver == testTeam.SecondDriver
        );
    }

    [Test]
    public async Task CreateOrUpdateTeamAsync_Should_UpdateExistingTeam()
    {
        var updatedTeam = new F1Team(testTeam.Name, "UpdatedDriver1", "UpdatedDriver2");
        await F1PredictionsService.CreateOrUpdateTeamAsync(updatedTeam);

        var teams = await F1PredictionsService.GetActiveTeamsAsync();

        teams.Should().Contain(t =>
            t.Name == testTeam.Name
            && t.FirstDriver == "UpdatedDriver1"
            && t.SecondDriver == "UpdatedDriver2"
        );
    }

    [Test]
    public async Task StartNewRaceAsync_Should_CreateActiveOpenedRace()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Monaco GP", false);

        var race = await F1PredictionsService.ReadAsync(raceId);

        race.Should().NotBeNull();
        race.Name.Should().Be("Monaco GP");
        race.IsActive.Should().BeTrue();
        race.IsOpened.Should().BeTrue();
        race.IsSprint.Should().BeFalse();
        race.Season.Should().Be(DateTime.UtcNow.Year);
    }

    [Test]
    public async Task StartNewRaceAsync_Should_CreateSprintRace()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Monaco Sprint", true);

        var race = await F1PredictionsService.ReadAsync(raceId);

        race.IsSprint.Should().BeTrue();
    }

    [Test]
    public async Task StartNewRaceAsync_Should_AssignPositionPredictionDriver()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("British GP", false);

        var race = await F1PredictionsService.ReadAsync(raceId);

        race.Conditions!.PositionPredictionDriver.Should().BeOneOf("Driver1", "Driver2");
    }

    [Test]
    public async Task ReadActiveAsync_Should_ContainNewlyCreatedRace()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Active Race", false);

        var activeRaces = await F1PredictionsService.ReadActiveAsync();

        activeRaces.Should().Contain(r => r.Id == raceId);
    }

    [Test]
    public async Task FindAsync_Should_FilterBySeason()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Singapore GP", false);

        var found = await F1PredictionsService.FindAsync(
            new F1RaceFilter
            {
                Season = DateTime.UtcNow.Year,
                IsActive = true,
            }
        );

        found.Should().Contain(r => r.Id == raceId);
    }

    [Test]
    public async Task AddPredictionAsync_Should_AddPrediction()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Italian GP", false);
        var userId = Guid.NewGuid();

        await F1PredictionsService.AddPredictionAsync(raceId, userId, CreatePrediction(userId));

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.Predictions.Should().ContainSingle(p => p.UserId == userId);
    }

    [Test]
    public async Task AddPredictionAsync_Should_UpdateExistingPredictionForSameUser()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("German GP", false);
        var userId = Guid.NewGuid();

        await F1PredictionsService.AddPredictionAsync(raceId, userId, CreatePrediction(userId));
        await F1PredictionsService.AddPredictionAsync(raceId, userId, CreatePrediction(userId));

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.Predictions.Should().ContainSingle(p => p.UserId == userId);
    }

    [Test]
    public async Task AddPredictionAsync_Should_Throw_WhenPredictionsAreClosed()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Closed Race", false);
        await F1PredictionsService.ClosePredictionsAsync(raceId);

        var act = () => F1PredictionsService.AddPredictionAsync(raceId, Guid.NewGuid(), CreatePrediction(Guid.NewGuid()));

        await act.Should().ThrowAsync<PredictionsAlreadyClosedException>();
    }

    [Test]
    public async Task ClosePredictionsAsync_Should_CloseRaceForPredictions_ButKeepItActive()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Japanese GP", false);

        await F1PredictionsService.ClosePredictionsAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.IsOpened.Should().BeFalse();
        race.IsActive.Should().BeTrue();
    }

    [Test]
    public async Task AddRaceResultAsync_Should_PersistResult()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Austrian GP", false);
        var raceResult = new F1PredictionRaceResult
        {
            RaceId = raceId,
            Classification = ["Driver1", "Driver2"],
            DnfDrivers = [],
            SafetyCars = 1,
            FirstPlaceLead = 7.5m,
        };

        await F1PredictionsService.AddRaceResultAsync(raceId, raceResult);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.Result.SafetyCars.Should().Be(1);
        race.Result.FirstPlaceLead.Should().Be(7.5m);
        race.Result.Classification.Should().BeEquivalentTo("Driver1", "Driver2");
    }

    [Test]
    public async Task FinishRaceAsync_Should_MarkRaceAsInactive()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Dutch GP", false);
        var race = await F1PredictionsService.ReadAsync(raceId);
        await SetupRaceForFinish(raceId, race.Conditions!.PositionPredictionDriver);

        await F1PredictionsService.FinishRaceAsync(raceId);

        var finishedRace = await F1PredictionsService.ReadAsync(raceId);
        finishedRace.IsActive.Should().BeFalse();
        finishedRace.IsOpened.Should().BeFalse();
    }

    [Test]
    public async Task FinishRaceAsync_Should_PersistPredictionResults()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Abu Dhabi GP", false);
        var race = await F1PredictionsService.ReadAsync(raceId);
        var userId = await SetupRaceForFinish(raceId, race.Conditions!.PositionPredictionDriver);

        var results = await F1PredictionsService.FinishRaceAsync(raceId);

        results.Should().ContainSingle(r => r.UserId == userId);

        var storedResults = await F1PredictionsService.ReadRaceResultsAsync(raceId);
        storedResults.Should().ContainSingle(r => r.UserId == userId);
    }

    [Test]
    public async Task FinishRaceAsync_Should_ReturnEmptyResults_WhenNoPredictions()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Empty Race GP", false);
        await F1PredictionsService.AddRaceResultAsync(
            raceId, new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = ["Driver1"],
                DnfDrivers = [],
                SafetyCars = 0,
                FirstPlaceLead = 0m,
            }
        );

        var results = await F1PredictionsService.FinishRaceAsync(raceId);

        results.Should().BeEmpty();
    }

    [Test]
    public async Task ReadActiveAsync_Should_NotContainFinishedRace()
    {
        var raceId = await F1PredictionsService.StartNewRaceAsync("Finished Race GP", false);
        var race = await F1PredictionsService.ReadAsync(raceId);
        await SetupRaceForFinish(raceId, race.Conditions!.PositionPredictionDriver);
        await F1PredictionsService.FinishRaceAsync(raceId);

        var activeRaces = await F1PredictionsService.ReadActiveAsync();

        activeRaces.Should().NotContain(r => r.Id == raceId);
    }

    private async Task<Guid> SetupRaceForFinish(Guid raceId, string positionDriver)
    {
        var userId = Guid.NewGuid();
        await F1PredictionsService.AddPredictionAsync(raceId, userId, CreatePrediction(userId));
        await F1PredictionsService.AddRaceResultAsync(
            raceId, new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = [positionDriver, "OtherDriver"],
                DnfDrivers = [],
                SafetyCars = 0,
                FirstPlaceLead = 5.0m,
            }
        );
        return userId;
    }

    private static F1Prediction CreatePrediction(Guid userId)
    {
        return new F1Prediction
        {
            UserId = userId,
            TenthPlacePickedDriver = "Driver1",
            DnfPrediction = new DnfPrediction { NoDnfPredicted = true },
            SafetyCarsPrediction = SafetyCarsCount.Zero,
            DriverPositionPrediction = 5,
            FirstPlaceLeadPrediction = 5.0m,
        };
    }

    private F1Team testTeam = null!;
}