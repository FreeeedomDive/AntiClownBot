using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using FluentAssertions;
using NSubstitute;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1QualifyingGridServiceTests : IntegrationTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        JolpicaClientMock.ClearReceivedCalls();
        JolpicaClientMock
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(null));

        testTeam = new F1Team("TestTeam", "Driver1", "Driver2");
        await F1PredictionsService.CreateOrUpdateTeamAsync(testTeam);
        raceId = await F1PredictionsService.StartNewRaceAsync("Qualifying Test GP", false);
    }

    [Test]
    public async Task SaveQualifyingGridAsync_Should_PersistGridToDatabase()
    {
        var grid = new[] { "Driver2", "Driver1" };

        await F1PredictionsService.SaveQualifyingGridAsync(raceId, grid);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().ContainInOrder("Driver2", "Driver1");
    }

    [Test]
    public async Task SaveQualifyingGridAsync_Should_OverwriteExistingGrid()
    {
        await F1PredictionsService.SaveQualifyingGridAsync(raceId, ["Driver1", "Driver2"]);
        await F1PredictionsService.SaveQualifyingGridAsync(raceId, ["Driver2", "Driver1"]);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().ContainInOrder("Driver2", "Driver1");
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotSaveGrid_WhenJolpicaReturnsNull()
    {
        JolpicaClientMock
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(null));

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().BeNull();
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_NotSaveGrid_WhenJolpicaReturnsEmpty()
    {
        JolpicaClientMock
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>([]));

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().BeNull();
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_SaveGrid_WhenJolpicaReturnsDriverNames()
    {
        JolpicaClientMock
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(["Driver1", "Driver2"]));

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().NotBeNull();
        race.QualifyingGrid.Should().ContainInOrder("Driver1", "Driver2");
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_AppendMissingTeamDrivers_WhenJolpicaGridIsPartial()
    {
        JolpicaClientMock
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromResult<string[]?>(["Driver1"]));

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        var race = await F1PredictionsService.ReadAsync(raceId);
        race.QualifyingGrid.Should().NotBeNull();
        race.QualifyingGrid![0].Should().Be("Driver1");
        race.QualifyingGrid.Should().Contain("Driver2");
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_ExcludeSprintRaces_WhenCalculatingRoundIndex()
    {
        await F1PredictionsService.StartNewRaceAsync("Sprint Race", true);
        var secondRaceId = await F1PredictionsService.StartNewRaceAsync("Second GP", false);

        var capturedRoundIndex = -1;
        JolpicaClientMock
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Is<int>(i => true))
            .Returns(callInfo =>
                {
                    capturedRoundIndex = callInfo.ArgAt<int>(1);
                    return Task.FromResult<string[]?>(["Driver1"]);
                }
            );

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        capturedRoundIndex.Should().Be(1);
    }

    [Test]
    public async Task PollQualifyingGridAsync_Should_CallJolpicaWithCurrentSeason()
    {
        var capturedSeason = -1;
        JolpicaClientMock
            .GetQualifyingDriverNamesAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(callInfo =>
                {
                    capturedSeason = callInfo.ArgAt<int>(0);
                    return Task.FromResult<string[]?>(null);
                }
            );

        await F1PredictionsService.PollQualifyingGridAsync(raceId);

        capturedSeason.Should().Be(DateTime.UtcNow.Year);
    }

    private Guid raceId;
    private F1Team testTeam = null!;
}