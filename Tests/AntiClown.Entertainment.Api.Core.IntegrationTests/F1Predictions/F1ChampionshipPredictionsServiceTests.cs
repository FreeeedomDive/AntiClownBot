using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;
using FluentAssertions;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1ChampionshipPredictionsServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task ReadResultsAsync_Should_ReturnResultsWithNoData_WhenNoneWritten()
    {
        var results = await F1ChampionshipPredictionsService.ReadResultsAsync(EmptySeason);

        results.HasData.Should().BeFalse();
    }

    [Test]
    public async Task WriteResultsAsync_Should_PersistResults()
    {
        var results = new F1ChampionshipResults
        {
            HasData = true,
            Stage = F1ChampionshipPredictionType.PreSeason,
            IsOpen = true,
            Standings = TestStandings,
        };

        await F1ChampionshipPredictionsService.WriteResultsAsync(ResultsSeason, results);

        var stored = await F1ChampionshipPredictionsService.ReadResultsAsync(ResultsSeason);
        stored.HasData.Should().BeTrue();
        stored.IsOpen.Should().BeTrue();
        stored.Stage.Should().Be(F1ChampionshipPredictionType.PreSeason);
        stored.Standings.Should().BeEquivalentTo(TestStandings);
    }

    [Test]
    public async Task WriteResultsAsync_Should_OverwriteExistingResults()
    {
        await F1ChampionshipPredictionsService.WriteResultsAsync(
            ResultsSeason, new F1ChampionshipResults
            {
                HasData = true,
                Stage = F1ChampionshipPredictionType.PreSeason,
                IsOpen = true,
                Standings = TestStandings,
            }
        );

        await F1ChampionshipPredictionsService.WriteResultsAsync(
            ResultsSeason, new F1ChampionshipResults
            {
                HasData = true,
                Stage = F1ChampionshipPredictionType.MidSeason,
                IsOpen = false,
                Standings = ["HAM", "VER"],
            }
        );

        var stored = await F1ChampionshipPredictionsService.ReadResultsAsync(ResultsSeason);
        stored.Stage.Should().Be(F1ChampionshipPredictionType.MidSeason);
        stored.IsOpen.Should().BeFalse();
        stored.Standings.Should().BeEquivalentTo("HAM", "VER");
    }

    [Test]
    public async Task CreateOrUpdateAsync_Should_Throw_WhenResultsHaveNoData()
    {
        var prediction = new F1ChampionshipPrediction
        {
            Season = ClosedSeason,
            UserId = Guid.NewGuid(),
            PreSeasonStandings = TestStandings,
        };

        var act = () => F1ChampionshipPredictionsService.CreateOrUpdateAsync(prediction);

        await act.Should().ThrowAsync<ChampionshipPredictionsClosedException>();
    }

    [Test]
    public async Task CreateOrUpdateAsync_Should_Throw_WhenPredictionsAreClosed()
    {
        await F1ChampionshipPredictionsService.WriteResultsAsync(
            ClosedSeason, new F1ChampionshipResults
            {
                HasData = true,
                Stage = F1ChampionshipPredictionType.PreSeason,
                IsOpen = false,
                Standings = TestStandings,
            }
        );

        var act = () => F1ChampionshipPredictionsService.CreateOrUpdateAsync(
            new F1ChampionshipPrediction
            {
                Season = ClosedSeason,
                UserId = Guid.NewGuid(),
                PreSeasonStandings = TestStandings,
            }
        );

        await act.Should().ThrowAsync<ChampionshipPredictionsClosedException>();
    }

    [Test]
    public async Task CreateOrUpdateAsync_Should_CreatePrediction_WhenOpen()
    {
        await F1ChampionshipPredictionsService.WriteResultsAsync(
            OpenSeason, new F1ChampionshipResults
            {
                HasData = true,
                Stage = F1ChampionshipPredictionType.PreSeason,
                IsOpen = true,
                Standings = TestStandings,
            }
        );

        var userId = Guid.NewGuid();
        var prediction = new F1ChampionshipPrediction
        {
            Season = OpenSeason,
            UserId = userId,
            PreSeasonStandings = TestStandings,
        };

        await F1ChampionshipPredictionsService.CreateOrUpdateAsync(prediction);

        var stored = await F1ChampionshipPredictionsService.ReadAsync(userId, OpenSeason);
        stored.UserId.Should().Be(userId);
        stored.Season.Should().Be(OpenSeason);
        stored.PreSeasonStandings.Should().BeEquivalentTo(TestStandings);
    }

    [Test]
    public async Task CreateOrUpdateAsync_Should_UpdateExistingPrediction()
    {
        await F1ChampionshipPredictionsService.WriteResultsAsync(
            OpenSeason, new F1ChampionshipResults
            {
                HasData = true,
                Stage = F1ChampionshipPredictionType.PreSeason,
                IsOpen = true,
                Standings = TestStandings,
            }
        );

        var userId = Guid.NewGuid();
        await F1ChampionshipPredictionsService.CreateOrUpdateAsync(
            new F1ChampionshipPrediction
            {
                Season = OpenSeason,
                UserId = userId,
                PreSeasonStandings = ["VER", "NOR"],
            }
        );

        await F1ChampionshipPredictionsService.CreateOrUpdateAsync(
            new F1ChampionshipPrediction
            {
                Season = OpenSeason,
                UserId = userId,
                PreSeasonStandings = ["NOR", "VER"],
            }
        );

        var stored = await F1ChampionshipPredictionsService.ReadAsync(userId, OpenSeason);
        stored.PreSeasonStandings.Should().BeEquivalentTo("NOR", "VER");
    }

    [Test]
    public async Task BuildPointsAsync_Should_ReturnPointsForAllPredictions()
    {
        await F1ChampionshipPredictionsService.WriteResultsAsync(
            PointsSeason, new F1ChampionshipResults
            {
                HasData = true,
                Stage = F1ChampionshipPredictionType.PreSeason,
                IsOpen = true,
                Standings = TestStandings,
            }
        );

        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        await F1ChampionshipPredictionsService.CreateOrUpdateAsync(
            new F1ChampionshipPrediction
            {
                Season = PointsSeason,
                UserId = user1,
                PreSeasonStandings = TestStandings,
            }
        );
        await F1ChampionshipPredictionsService.CreateOrUpdateAsync(
            new F1ChampionshipPrediction
            {
                Season = PointsSeason,
                UserId = user2,
                PreSeasonStandings = ["NOR", "VER", "LEC", "HAM", "RUS"],
            }
        );

        var points = await F1ChampionshipPredictionsService.BuildPointsAsync(PointsSeason);

        points.Should().HaveCount(2);
        points.Should().Contain(p => p.UserId == user1);
        points.Should().Contain(p => p.UserId == user2);
    }

    [Test]
    public async Task BuildPointsAsync_Should_ReturnEmpty_WhenNoPredictions()
    {
        await F1ChampionshipPredictionsService.WriteResultsAsync(
            PointsSeason, new F1ChampionshipResults
            {
                HasData = true,
                Stage = F1ChampionshipPredictionType.PreSeason,
                IsOpen = true,
                Standings = TestStandings,
            }
        );

        var points = await F1ChampionshipPredictionsService.BuildPointsAsync(PointsSeason);

        points.Should().BeEmpty();
    }

    private const int ClosedSeason = 9971;
    private const int OpenSeason = 9972;
    private const int ResultsSeason = 9973;
    private const int PointsSeason = 9974;
    private const int EmptySeason = 9975;

    private static readonly string[] TestStandings = ["VER", "NOR", "LEC", "HAM", "RUS"];
}