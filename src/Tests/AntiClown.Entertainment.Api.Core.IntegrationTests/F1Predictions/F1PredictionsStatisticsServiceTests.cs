using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using FluentAssertions;
using NSubstitute;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1PredictionsStatisticsServiceTests : IntegrationTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        testTeam = new F1Team("StatsTestTeam", PositionDriver, "ReserveDriver");
        await F1PredictionsService.CreateOrUpdateTeamAsync(testTeam);
    }

    [Test]
    public async Task GetSeasonStatsAsync_Should_ReturnEmptyStats_WhenNoRacesExistForSeason()
    {
        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9900);

        stats.TenthPlacePointsRating.Should().BeEmpty();
        stats.MostPickedForTenthPlace.Should().BeEmpty();
        stats.TenthPickedButDnfed.Should().BeEmpty();
        stats.DriverOwnTenthPlacePoints.Should().BeEmpty();
        stats.TenthPlacePredictionEfficiency.Should().BeEmpty();
        stats.MostDnfDrivers.Should().BeEmpty();
        stats.MostPickedForDnf.Should().BeEmpty();
        stats.TenthPlaceDnfAntiRating.Should().BeEmpty();
        stats.ClosestLeadGapPredictions.Should().BeEmpty();
        stats.SafetyCarPickCounts.Should().BeEmpty();
        stats.SafetyCarActualCounts.Should().BeEmpty();
        stats.SafetyCarCorrectGuesses.Should().BeEmpty();
    }

    [Test]
    public async Task GetSeasonStatsAsync_Should_ReturnEmptyStats_WhenAllRacesAreStillActive()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(9901, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Active Race GP", false);
        var userId = Guid.NewGuid();
        await F1PredictionsService.AddPredictionAsync(raceId, userId, CreatePrediction(userId, "DriverA"));

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9901);

        stats.TenthPlacePointsRating.Should().BeEmpty();
        stats.MostPickedForTenthPlace.Should().BeEmpty();
    }

    [Test]
    public async Task GetSeasonStatsAsync_Should_NotMixDataBetweenSeasons()
    {
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        await CreateFinishedRace(
            9902,
            predictions: [
                (user1, CreatePrediction(user1, "SeasonADriver")),
            ]
        );
        await CreateFinishedRace(
            9903,
            predictions: [
                (user2, CreatePrediction(user2, "SeasonBDriver")),
            ]
        );

        var statsA = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9902);
        var statsB = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9903);

        statsA.MostPickedForTenthPlace.Should().ContainSingle(x => x.Driver == "SeasonADriver");
        statsA.MostPickedForTenthPlace.Should().NotContain(x => x.Driver == "SeasonBDriver");
        statsB.MostPickedForTenthPlace.Should().ContainSingle(x => x.Driver == "SeasonBDriver");
        statsB.MostPickedForTenthPlace.Should().NotContain(x => x.Driver == "SeasonADriver");
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlacePointsRating_Should_SumPointsAcrossRacesForSameDriver()
    {
        // Race 1: DriverA финишировал 10-м (25 pts), DriverB — 11-м (18 pts)
        // Race 2: DriverA финишировал 10-м (25 pts)
        // Итого: DriverA = 50, DriverB = 18
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();
        var classificationWithBoth = new[]
        {
            "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9",
            "DriverA", "DriverB", PositionDriver,
        };
        await CreateFinishedRace(
            9910,
            classification: classificationWithBoth,
            predictions: [
                (user1, CreatePrediction(user1, "DriverA")),
                (user2, CreatePrediction(user2, "DriverB")),
            ]
        );
        await CreateFinishedRace(
            9910,
            classification: BuildClassification("DriverA"),
            predictions: [
                (user3, CreatePrediction(user3, "DriverA")),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9910);

        stats.TenthPlacePointsRating[0].Driver.Should().Be("DriverA");
        stats.TenthPlacePointsRating[0].Score.Should().Be(50);
        stats.TenthPlacePointsRating[1].Driver.Should().Be("DriverB");
        stats.TenthPlacePointsRating[1].Score.Should().Be(18);
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlacePointsRating_Should_NotIncludeDriversWithZeroPoints()
    {
        // DriverZ финишировал вне очковой зоны (позиция 21+)
        var user1 = Guid.NewGuid();
        var classification = new[]
        {
            "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10",
            "P11", "P12", "P13", "P14", "P15", "P16", "P17", "P18", "P19", "P20",
            "P21", PositionDriver,
        };
        await CreateFinishedRace(
            9911,
            classification: classification,
            predictions: [
                (user1, CreatePrediction(user1, "DriverZ")),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9911);

        stats.TenthPlacePointsRating.Should().NotContain(x => x.Driver == "DriverZ");
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlacePointsRating_Should_ReturnAtMostTop5()
    {
        var users = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToArray();
        var drivers = Enumerable.Range(1, 6).Select(i => $"TopDriver{i}").ToArray();
        var classification = new[] { drivers[0], drivers[1], drivers[2], drivers[3], drivers[4], drivers[5], PositionDriver };
        var predictions = users.Zip(drivers, (u, d) => (u, CreatePrediction(u, d))).ToArray();
        await CreateFinishedRace(9912, classification: classification, predictions: predictions);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9912);

        stats.TenthPlacePointsRating.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostPickedForTenthPlace_Should_CountPicksByDriver()
    {
        var users = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(
            9920,
            predictions: [
                (users[0], CreatePrediction(users[0], "PopularDriver")),
                (users[1], CreatePrediction(users[1], "PopularDriver")),
                (users[2], CreatePrediction(users[2], "RareDriver")),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9920);

        stats.MostPickedForTenthPlace[0].Driver.Should().Be("PopularDriver");
        stats.MostPickedForTenthPlace[0].Score.Should().Be(2);
        stats.MostPickedForTenthPlace[1].Driver.Should().Be("RareDriver");
        stats.MostPickedForTenthPlace[1].Score.Should().Be(1);
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostPickedForTenthPlace_Should_AccumulateAcrossRaces()
    {
        var users = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(
            9921,
            predictions: [
                (users[0], CreatePrediction(users[0], "DriverX")),
                (users[1], CreatePrediction(users[1], "DriverX")),
            ]
        );
        await CreateFinishedRace(
            9921,
            predictions: [
                (users[2], CreatePrediction(users[2], "DriverX")),
                (users[3], CreatePrediction(users[3], "DriverY")),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9921);

        stats.MostPickedForTenthPlace.First(x => x.Driver == "DriverX").Score.Should().Be(3);
        stats.MostPickedForTenthPlace.First(x => x.Driver == "DriverY").Score.Should().Be(1);
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostPickedForTenthPlace_Should_ReturnAtMostTop5()
    {
        var users = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToArray();
        var drivers = Enumerable.Range(1, 6).Select(i => $"Tenth{i}").ToArray();
        var predictions = users.Zip(drivers, (u, d) => (u, CreatePrediction(u, d))).ToArray();
        await CreateFinishedRace(9922, predictions: predictions);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9922);

        stats.MostPickedForTenthPlace.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPickedButDnfed_Should_CountDriverPickedForTenthButDnfed()
    {
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        await CreateFinishedRace(
            9930,
            dnfDrivers: ["DnfDriver"],
            predictions: [
                (user1, CreatePrediction(user1, "DnfDriver")),
                (user2, CreatePrediction(user2, "FinishedDriver")),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9930);

        stats.TenthPickedButDnfed.Should().ContainSingle(x => x.Driver == "DnfDriver" && x.Score == 1);
        stats.TenthPickedButDnfed.Should().NotContain(x => x.Driver == "FinishedDriver");
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPickedButDnfed_Should_AccumulateAcrossRaces()
    {
        var users = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(
            9931,
            dnfDrivers: ["UnluckyDriver"],
            predictions: [
                (users[0], CreatePrediction(users[0], "UnluckyDriver")),
            ]
        );
        await CreateFinishedRace(
            9931,
            dnfDrivers: ["UnluckyDriver"],
            predictions: [
                (users[1], CreatePrediction(users[1], "UnluckyDriver")),
                (users[2], CreatePrediction(users[2], "OtherDriver")),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9931);

        stats.TenthPickedButDnfed.First(x => x.Driver == "UnluckyDriver").Score.Should().Be(2);
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPickedButDnfed_Should_ReturnAtMostTop5()
    {
        var users = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToArray();
        var drivers = Enumerable.Range(1, 6).Select(i => $"DnfPick{i}").ToArray();
        var predictions = users.Zip(drivers, (u, d) => (u, CreatePrediction(u, d))).ToArray();
        await CreateFinishedRace(9932, dnfDrivers: drivers, predictions: predictions);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9932);

        stats.TenthPickedButDnfed.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostDnfDrivers_Should_CountActualDnfs()
    {
        var user1 = Guid.NewGuid();
        await CreateFinishedRace(
            9940,
            dnfDrivers: ["CrashKing", "BackmarkerDnf"],
            predictions: [(user1, CreatePrediction(user1, "SomeDriver"))]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9940);

        stats.MostDnfDrivers.Should().Contain(x => x.Driver == "CrashKing" && x.Score == 1);
        stats.MostDnfDrivers.Should().Contain(x => x.Driver == "BackmarkerDnf" && x.Score == 1);
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostDnfDrivers_Should_AccumulateAcrossRaces()
    {
        var users = Enumerable.Range(0, 2).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(
            9941,
            dnfDrivers: ["RepeatedDnf"],
            predictions: [(users[0], CreatePrediction(users[0], "SomeDriver"))]
        );
        await CreateFinishedRace(
            9941,
            dnfDrivers: ["RepeatedDnf", "OnceDnf"],
            predictions: [(users[1], CreatePrediction(users[1], "SomeDriver"))]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9941);

        stats.MostDnfDrivers.First(x => x.Driver == "RepeatedDnf").Score.Should().Be(2);
        stats.MostDnfDrivers.First(x => x.Driver == "OnceDnf").Score.Should().Be(1);
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostDnfDrivers_Should_BeEmptyWhenNoDnfsOccurred()
    {
        var user1 = Guid.NewGuid();
        await CreateFinishedRace(
            9942,
            dnfDrivers: [],
            predictions: [(user1, CreatePrediction(user1, "SomeDriver"))]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9942);

        stats.MostDnfDrivers.Should().BeEmpty();
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostDnfDrivers_Should_ReturnAtMostTop5()
    {
        var dnfDrivers = Enumerable.Range(1, 6).Select(i => $"DnfActor{i}").ToArray();
        var user1 = Guid.NewGuid();
        await CreateFinishedRace(
            9943,
            dnfDrivers: dnfDrivers,
            predictions: [(user1, CreatePrediction(user1, "SomeDriver"))]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9943);

        stats.MostDnfDrivers.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostPickedForDnf_Should_CountDnfPredictions()
    {
        var users = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(
            9950,
            predictions: [
                (users[0], CreatePredictionWithDnf(users[0], ["DnfFavorite", "DnfRare"])),
                (users[1], CreatePredictionWithDnf(users[1], ["DnfFavorite"])),
                (users[2], CreatePrediction(users[2], "SomeDriver")), // NoDnf предсказание — не должно считаться
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9950);

        stats.MostPickedForDnf.First(x => x.Driver == "DnfFavorite").Score.Should().Be(2);
        stats.MostPickedForDnf.First(x => x.Driver == "DnfRare").Score.Should().Be(1);
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostPickedForDnf_Should_NotCountNoDnfPredictions()
    {
        var users = Enumerable.Range(0, 2).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(
            9951,
            predictions: [
                (users[0], CreatePrediction(users[0], "Driver1")), // NoDnfPredicted = true
                (users[1], CreatePrediction(users[1], "Driver2")), // NoDnfPredicted = true
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9951);

        stats.MostPickedForDnf.Should().BeEmpty();
    }

    [Test]
    public async Task GetSeasonStatsAsync_MostPickedForDnf_Should_ReturnAtMostTop5()
    {
        var users = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToArray();
        var dnfDrivers = Enumerable.Range(1, 6).Select(i => $"DnfPicked{i}").ToArray();
        var predictions = users
            .Zip(dnfDrivers, (u, d) => (u, CreatePredictionWithDnf(u, [d])))
            .ToArray();
        await CreateFinishedRace(9952, predictions: predictions);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9952);

        stats.MostPickedForDnf.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_ClosestLeadGapPredictions_Should_OrderByAscendingDifference()
    {
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();
        await CreateFinishedRace(
            9960,
            firstPlaceLead: 5.0m,
            predictions: [
                (user1, CreatePredictionWithLead(user1, 4.5m)),  // diff = 0.5
                (user2, CreatePredictionWithLead(user2, 5.1m)),  // diff = 0.1 ← ближе всех
                (user3, CreatePredictionWithLead(user3, 3.0m)),  // diff = 2.0
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9960);

        stats.ClosestLeadGapPredictions[0].UserId.Should().Be(user2);
        stats.ClosestLeadGapPredictions[0].Difference.Should().Be(0.1m);
        stats.ClosestLeadGapPredictions[1].UserId.Should().Be(user1);
        stats.ClosestLeadGapPredictions[1].Difference.Should().Be(0.5m);
        stats.ClosestLeadGapPredictions[2].UserId.Should().Be(user3);
        stats.ClosestLeadGapPredictions[2].Difference.Should().Be(2.0m);
    }

    [Test]
    public async Task GetSeasonStatsAsync_ClosestLeadGapPredictions_Should_PopulateAllFields()
    {
        var user1 = Guid.NewGuid();
        await CreateFinishedRace(
            9961,
            firstPlaceLead: 7.5m,
            predictions: [(user1, CreatePredictionWithLead(user1, 7.0m))], raceName: "Monaco GP"
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9961);

        var entry = stats.ClosestLeadGapPredictions.Should().ContainSingle().Subject;
        entry.UserId.Should().Be(user1);
        entry.RaceName.Should().Be("Monaco GP");
        entry.PredictedGap.Should().Be(7.0m);
        entry.ActualGap.Should().Be(7.5m);
        entry.Difference.Should().Be(0.5m);
    }

    [Test]
    public async Task GetSeasonStatsAsync_ClosestLeadGapPredictions_Should_AggregateAcrossRaces()
    {
        var users = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(
            9962,
            firstPlaceLead: 10.0m,
            predictions: [(users[0], CreatePredictionWithLead(users[0], 9.9m))] // diff = 0.1
        );
        await CreateFinishedRace(
            9962,
            firstPlaceLead: 3.0m,
            predictions: [
                (users[1], CreatePredictionWithLead(users[1], 2.8m)),  // diff = 0.2
                (users[2], CreatePredictionWithLead(users[2], 4.0m)),  // diff = 1.0
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9962);

        stats.ClosestLeadGapPredictions[0].UserId.Should().Be(users[0]);
        stats.ClosestLeadGapPredictions[1].UserId.Should().Be(users[1]);
        stats.ClosestLeadGapPredictions[2].UserId.Should().Be(users[2]);
    }

    [Test]
    public async Task GetSeasonStatsAsync_ClosestLeadGapPredictions_Should_ReturnAtMostTop5()
    {
        var users = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToArray();
        var leads = new[] { 1.0m, 2.0m, 3.0m, 4.0m, 5.0m, 6.0m };
        var predictions = users.Zip(leads, (u, l) => (u, CreatePredictionWithLead(u, l))).ToArray();
        await CreateFinishedRace(9963, firstPlaceLead: 0m, predictions: predictions);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9963);

        stats.ClosestLeadGapPredictions.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_DriverOwnTenthPlacePoints_Should_SumPointsByPositionAcrossRaces()
    {
        // Race 1: DriverA на 10-м (25 pts), DriverB на 11-м (18 pts)
        // Race 2: DriverA снова на 10-м (25 pts)
        // Итого: DriverA = 50, DriverB = 18 — DriverB занимает 2 место (выше разовых 15-pt филлеров)
        var race1Classification = new[] { PositionDriver }
            .Concat(Enumerable.Range(1, 8).Select(i => $"R9964A_F{i}"))
            .Concat(["DriverA", "DriverB"])
            .ToArray();
        var race2Classification = new[] { PositionDriver }
            .Concat(Enumerable.Range(1, 8).Select(i => $"R9964B_F{i}"))
            .Append("DriverA")
            .ToArray();

        await CreateFinishedRace(9964, classification: race1Classification);
        await CreateFinishedRace(9964, classification: race2Classification);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9964);

        var driverAEntry = stats.DriverOwnTenthPlacePoints.Should().Contain(x => x.Driver == "DriverA").Subject;
        driverAEntry.Score.Should().Be(50);
        var driverBEntry = stats.DriverOwnTenthPlacePoints.Should().Contain(x => x.Driver == "DriverB").Subject;
        driverBEntry.Score.Should().Be(18);
        var indexA = Array.IndexOf(stats.DriverOwnTenthPlacePoints, driverAEntry);
        var indexB = Array.IndexOf(stats.DriverOwnTenthPlacePoints, driverBEntry);
        indexA.Should().BeLessThan(indexB);
    }

    [Test]
    public async Task GetSeasonStatsAsync_DriverOwnTenthPlacePoints_Should_UseCorrectScoringByPosition()
    {
        var classification = new[] { "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "DriverA", "DriverB", PositionDriver };
        await CreateFinishedRace(9965, classification: classification);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9965);

        stats.DriverOwnTenthPlacePoints[0].Driver.Should().Be("DriverB");
        stats.DriverOwnTenthPlacePoints[0].Score.Should().Be(25);
        stats.DriverOwnTenthPlacePoints[1].Driver.Should().Be("DriverA");
        stats.DriverOwnTenthPlacePoints[1].Score.Should().Be(18);
    }

    [Test]
    public async Task GetSeasonStatsAsync_DriverOwnTenthPlacePoints_Should_ExcludeDriversWithZeroPoints()
    {
        var classification = new[]
        {
            "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10",
            "P11", "P12", "P13", "P14", "P15", "P16", "P17", "P18", "P19", "P20",
            "DriverZ", PositionDriver,
        };
        await CreateFinishedRace(9966, classification: classification);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9966);

        stats.DriverOwnTenthPlacePoints.Should().NotContain(x => x.Driver == "DriverZ");
    }

    [Test]
    public async Task GetSeasonStatsAsync_DriverOwnTenthPlacePoints_Should_ReturnAtMostTop5()
    {
        var drivers = Enumerable.Range(1, 6).Select(i => $"OwnDriver{i}").ToArray();
        var classification = new[] { drivers[0], drivers[1], drivers[2], drivers[3], drivers[4], drivers[5], PositionDriver };
        await CreateFinishedRace(9967, classification: classification);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9967);

        stats.DriverOwnTenthPlacePoints.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlacePredictionEfficiency_Should_CalculateFloorOfAveragePoints()
    {
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();
        await CreateFinishedRace(9968,
            classification: ["P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "DriverA", PositionDriver],
            predictions: [(user1, CreatePrediction(user1, "DriverA"))]
        );
        await CreateFinishedRace(9968,
            classification: ["P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "DriverA", "DriverB", PositionDriver],
            predictions: [
                (user2, CreatePrediction(user2, "DriverA")),
                (user3, CreatePrediction(user3, "DriverB")),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9968);

        stats.TenthPlacePredictionEfficiency[0].Driver.Should().Be("DriverB");
        stats.TenthPlacePredictionEfficiency[0].Score.Should().Be(25);
        stats.TenthPlacePredictionEfficiency[1].Driver.Should().Be("DriverA");
        stats.TenthPlacePredictionEfficiency[1].Score.Should().Be(21);
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlacePredictionEfficiency_Should_ExcludeDriversWithZeroTotalPoints()
    {
        var user1 = Guid.NewGuid();
        var classification = new[]
        {
            "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10",
            "P11", "P12", "P13", "P14", "P15", "P16", "P17", "P18", "P19", "P20",
            "DriverZ", PositionDriver,
        };
        await CreateFinishedRace(9969, classification: classification,
            predictions: [(user1, CreatePrediction(user1, "DriverZ"))]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9969);

        stats.TenthPlacePredictionEfficiency.Should().NotContain(x => x.Driver == "DriverZ");
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlacePredictionEfficiency_Should_ReturnAtMostTop5()
    {
        var users = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToArray();
        var drivers = Enumerable.Range(1, 6).Select(i => $"EffDriver{i}").ToArray();
        // Все гонщики на позициях 1–6 → ненулевые очки
        var classification = new[] { drivers[0], drivers[1], drivers[2], drivers[3], drivers[4], drivers[5], PositionDriver };
        var predictions = users.Zip(drivers, (u, d) => (u, CreatePrediction(u, d))).ToArray();
        await CreateFinishedRace(9970, classification: classification, predictions: predictions);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9970);

        stats.TenthPlacePredictionEfficiency.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlaceDnfAntiRating_Should_CountUsersWhosePickDnfed()
    {
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        await CreateFinishedRace(9971,
            dnfDrivers: ["DnfDriver"],
            predictions: [
                (user1, CreatePrediction(user1, "DnfDriver")),
                (user2, CreatePrediction(user2, "FinishedDriver")),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9971);

        stats.TenthPlaceDnfAntiRating.Should().ContainSingle(x => x.UserId == user1 && x.Score == 1);
        stats.TenthPlaceDnfAntiRating.Should().NotContain(x => x.UserId == user2);
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlaceDnfAntiRating_Should_AccumulateAcrossRaces()
    {
        var user1 = Guid.NewGuid();
        await CreateFinishedRace(9972,
            dnfDrivers: ["UnluckyDriver"],
            predictions: [(user1, CreatePrediction(user1, "UnluckyDriver"))]
        );
        await CreateFinishedRace(9972,
            dnfDrivers: ["UnluckyDriver"],
            predictions: [(user1, CreatePrediction(user1, "UnluckyDriver"))]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9972);

        stats.TenthPlaceDnfAntiRating.Should().ContainSingle(x => x.UserId == user1 && x.Score == 2);
    }

    [Test]
    public async Task GetSeasonStatsAsync_TenthPlaceDnfAntiRating_Should_ReturnAtMostTop5()
    {
        var users = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToArray();
        var drivers = Enumerable.Range(1, 6).Select(i => $"AntiDriver{i}").ToArray();
        var predictions = users.Zip(drivers, (u, d) => (u, CreatePrediction(u, d))).ToArray();
        await CreateFinishedRace(9973, dnfDrivers: drivers, predictions: predictions);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9973);

        stats.TenthPlaceDnfAntiRating.Should().HaveCount(5);
    }

    [Test]
    public async Task GetSeasonStatsAsync_SafetyCarPickCounts_Should_CountPicksPerVariant()
    {
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();
        await CreateFinishedRace(9974,
            predictions: [
                (user1, CreatePredictionWithSafetyCar(user1, SafetyCarsCount.Zero)),
                (user2, CreatePredictionWithSafetyCar(user2, SafetyCarsCount.Zero)),
                (user3, CreatePredictionWithSafetyCar(user3, SafetyCarsCount.One)),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9974);

        stats.SafetyCarPickCounts.Should().Contain(x => x.Driver == "Нет" && x.Score == 2);
        stats.SafetyCarPickCounts.Should().Contain(x => x.Driver == "1" && x.Score == 1);
    }

    [Test]
    public async Task GetSeasonStatsAsync_SafetyCarPickCounts_Should_AccumulateAcrossRaces()
    {
        var users = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(9975,
            predictions: [
                (users[0], CreatePredictionWithSafetyCar(users[0], SafetyCarsCount.Two)),
                (users[1], CreatePredictionWithSafetyCar(users[1], SafetyCarsCount.Two)),
            ]
        );
        await CreateFinishedRace(9975,
            predictions: [
                (users[2], CreatePredictionWithSafetyCar(users[2], SafetyCarsCount.Two)),
                (users[3], CreatePredictionWithSafetyCar(users[3], SafetyCarsCount.ThreePlus)),
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9975);

        stats.SafetyCarPickCounts.Should().Contain(x => x.Driver == "2" && x.Score == 3);
        stats.SafetyCarPickCounts.Should().Contain(x => x.Driver == "3+" && x.Score == 1);
    }

    [Test]
    public async Task GetSeasonStatsAsync_SafetyCarActualCounts_Should_CountActualVariantsAcrossRaces()
    {
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();
        await CreateFinishedRace(9976, safetyCars: 0, predictions: [(user1, CreatePrediction(user1, "D1"))]);
        await CreateFinishedRace(9976, safetyCars: 0, predictions: [(user2, CreatePrediction(user2, "D1"))]);
        await CreateFinishedRace(9976, safetyCars: 1, predictions: [(user3, CreatePrediction(user3, "D1"))]);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9976);

        stats.SafetyCarActualCounts.Should().Contain(x => x.Driver == "Нет" && x.Score == 2);
        stats.SafetyCarActualCounts.Should().Contain(x => x.Driver == "1" && x.Score == 1);
    }

    [Test]
    public async Task GetSeasonStatsAsync_SafetyCarActualCounts_Should_MapThreePlusCorrectly()
    {
        var user1 = Guid.NewGuid();
        await CreateFinishedRace(9977, safetyCars: 3, predictions: [(user1, CreatePrediction(user1, "D1"))]);
        await CreateFinishedRace(9977, safetyCars: 5, predictions: [(user1, CreatePrediction(user1, "D1"))]);

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9977);

        stats.SafetyCarActualCounts.Should().ContainSingle(x => x.Driver == "3+" && x.Score == 2);
    }

    [Test]
    public async Task GetSeasonStatsAsync_SafetyCarCorrectGuesses_Should_CountOnlyCorrectGuesses()
    {
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();
        await CreateFinishedRace(9978, safetyCars: 0,
            predictions: [
                (user1, CreatePredictionWithSafetyCar(user1, SafetyCarsCount.Zero)),  // угадал
                (user2, CreatePredictionWithSafetyCar(user2, SafetyCarsCount.Zero)),  // угадал
                (user3, CreatePredictionWithSafetyCar(user3, SafetyCarsCount.One)),   // не угадал
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9978);

        stats.SafetyCarCorrectGuesses.Should().ContainSingle(x => x.Driver == "Нет" && x.Score == 2);
        stats.SafetyCarCorrectGuesses.Should().NotContain(x => x.Driver == "1");
    }

    [Test]
    public async Task GetSeasonStatsAsync_SafetyCarCorrectGuesses_Should_AccumulateAcrossVariantsAndRaces()
    {
        var users = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToArray();
        await CreateFinishedRace(9979, safetyCars: 1,
            predictions: [
                (users[0], CreatePredictionWithSafetyCar(users[0], SafetyCarsCount.One)), // угадал
                (users[1], CreatePredictionWithSafetyCar(users[1], SafetyCarsCount.Two)), // не угадал
            ]
        );
        await CreateFinishedRace(9979, safetyCars: 2,
            predictions: [
                (users[2], CreatePredictionWithSafetyCar(users[2], SafetyCarsCount.Two)), // угадал
                (users[3], CreatePredictionWithSafetyCar(users[3], SafetyCarsCount.One)), // не угадал
            ]
        );

        var stats = await F1PredictionsStatisticsService.GetSeasonStatsAsync(9979);

        stats.SafetyCarCorrectGuesses.Should().Contain(x => x.Driver == "1" && x.Score == 1);
        stats.SafetyCarCorrectGuesses.Should().Contain(x => x.Driver == "2" && x.Score == 1);
        stats.SafetyCarCorrectGuesses.Should().NotContain(x => x.Driver == "Нет");
    }

    private async Task CreateFinishedRace(
        int season,
        string[]? classification = null,
        string[]? dnfDrivers = null,
        int safetyCars = 0,
        decimal firstPlaceLead = 5.0m,
        (Guid userId, F1Prediction prediction)[]? predictions = null,
        bool isSprint = false,
        string? raceName = null
    )
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(season, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var name = raceName ?? $"Test Race {Guid.NewGuid():N}";
        var raceId = await F1PredictionsService.StartNewRaceAsync(name, isSprint);

        foreach (var (userId, prediction) in predictions ?? [])
        {
            await F1PredictionsService.AddPredictionAsync(raceId, userId, prediction);
        }

        var finalClassification = classification ?? [PositionDriver, "FillDriver"];
        if (!finalClassification.Contains(PositionDriver))
        {
            finalClassification = [PositionDriver, ..finalClassification];
        }

        await F1PredictionsService.AddRaceResultAsync(raceId, new F1PredictionRaceResult
        {
            RaceId = raceId,
            Classification = finalClassification,
            DnfDrivers = dnfDrivers ?? [],
            SafetyCars = safetyCars,
            FirstPlaceLead = firstPlaceLead,
        });

        await F1PredictionsService.FinishRaceAsync(raceId);
    }

    private static string[] BuildClassification(string tenthDriver)
        =>
        [
            "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9",
            tenthDriver,
            PositionDriver,
        ];

    private static F1Prediction CreatePrediction(Guid userId, string tenthPlaceDriver) =>
        new()
        {
            UserId = userId,
            TenthPlacePickedDriver = tenthPlaceDriver,
            DnfPrediction = new DnfPrediction { NoDnfPredicted = true },
            SafetyCarsPrediction = SafetyCarsCount.Zero,
            DriverPositionPrediction = 1,
            FirstPlaceLeadPrediction = 5.0m,
        };

    private static F1Prediction CreatePredictionWithDnf(Guid userId, string[] dnfPickedDrivers) =>
        new()
        {
            UserId = userId,
            TenthPlacePickedDriver = "SomeTenthDriver",
            DnfPrediction = new DnfPrediction
            {
                NoDnfPredicted = false,
                DnfPickedDrivers = dnfPickedDrivers,
            },
            SafetyCarsPrediction = SafetyCarsCount.Zero,
            DriverPositionPrediction = 1,
            FirstPlaceLeadPrediction = 5.0m,
        };

    private static F1Prediction CreatePredictionWithLead(Guid userId, decimal leadPrediction) =>
        new()
        {
            UserId = userId,
            TenthPlacePickedDriver = "SomeTenthDriver",
            DnfPrediction = new DnfPrediction { NoDnfPredicted = true },
            SafetyCarsPrediction = SafetyCarsCount.Zero,
            DriverPositionPrediction = 1,
            FirstPlaceLeadPrediction = leadPrediction,
        };

    private static F1Prediction CreatePredictionWithSafetyCar(Guid userId, SafetyCarsCount safetyCars) =>
        new()
        {
            UserId = userId,
            TenthPlacePickedDriver = "SomeTenthDriver",
            DnfPrediction = new DnfPrediction { NoDnfPredicted = true },
            SafetyCarsPrediction = safetyCars,
            DriverPositionPrediction = 1,
            FirstPlaceLeadPrediction = 5.0m,
        };

    private const string PositionDriver = "StatsTestPositionDriver";

    private F1Team testTeam = null!;
}
