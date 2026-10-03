using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using FluentAssertions;
using NSubstitute;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1QualifyingGridServiceTests : IntegrationTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        StartingGridClientMock.ClearReceivedCalls();
        StartingGridClientMock
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(null));

        testTeam = new F1Team("TestTeam", "Driver1", "Driver2");
        await F1PredictionsService.CreateOrUpdateTeamAsync(testTeam);
    }

    [Test]
    public async Task SaveQualifyingGridAsync_Should_PersistGridToDatabase()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(2031, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);
        var grid = new[] { "Driver2", "Driver1" };

        await F1PredictionsService.SaveQualifyingGridAsync(raceId, grid);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().ContainInOrder("Driver2", "Driver1");
    }

    [Test]
    public async Task SaveQualifyingGridAsync_Should_OverwriteExistingGrid()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(2032, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);

        await F1PredictionsService.SaveQualifyingGridAsync(raceId, ["Driver1", "Driver2"]);
        await F1PredictionsService.SaveQualifyingGridAsync(raceId, ["Driver2", "Driver1"]);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().ContainInOrder("Driver2", "Driver1");
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotSaveGrid_WhenOpenF1ReturnsNull()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(2033, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);

        StartingGridClientMock
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(null));

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().BeNull();
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotSaveGrid_WhenOpenF1ReturnsEmpty()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(2034, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);

        StartingGridClientMock
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>([]));

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().BeNull();
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_SaveGrid_WhenOpenF1ReturnsDriverNames()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(2035, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);

        StartingGridClientMock
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(["Driver1", "Driver2"]));

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().NotBeNull();
        race.QualifyingGrid.Should().ContainInOrder("Driver1", "Driver2");
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotFabricateMissingTeamDrivers()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(2036, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);

        StartingGridClientMock
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string[]?>(["Driver1"]));

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().NotBeNull();
        race.QualifyingGrid![0].Should().Be("Driver1");
        race.QualifyingGrid.Should().NotContain("Driver2");
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_ExcludeSprintRaces_WhenCalculatingRoundIndex()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(2037, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);
        await F1PredictionsService.StartNewRaceAsync("Sprint Race", true);
        var secondRaceId = await F1PredictionsService.StartNewRaceAsync("Second GP", false);

        var capturedRoundIndex = -1;
        StartingGridClientMock
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Is<int>(i => true), Arg.Any<bool>())
            .Returns(callInfo =>
                {
                    capturedRoundIndex = callInfo.ArgAt<int>(1);
                    return Task.FromResult<string[]?>(["Driver1"]);
                }
            );

        await F1PredictionsService.PollQualifyingGridAsync(secondRaceId);

        capturedRoundIndex.Should().Be(2);
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_CallOpenF1WithCurrentSeason()
    {
        TimeProviderMock.GetUtcNow().Returns(new DateTimeOffset(2038, 6, 1, 0, 0, 0, TimeSpan.Zero));
        var raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);

        var capturedSeason = -1;
        StartingGridClientMock
            .GetDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .Returns(callInfo =>
                {
                    capturedSeason = callInfo.ArgAt<int>(0);
                    return Task.FromResult<string[]?>(null);
                }
            );

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        capturedSeason.Should().Be(2038);
    }

    private F1Team testTeam = null!;
}
