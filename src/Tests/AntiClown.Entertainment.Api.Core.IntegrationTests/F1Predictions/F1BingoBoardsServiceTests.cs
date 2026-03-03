using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using FluentAssertions;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1BingoBoardsServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task GetOrCreateBingoBoard_Should_CreateBoardWhenNoneExists()
    {
        var userId = Guid.NewGuid();

        var cardIds = await F1BingoBoardsService.GetOrCreateBingoBoard(userId, EmptySeason);

        cardIds.Should().NotBeNull();
    }

    [Test]
    public async Task GetOrCreateBingoBoard_Should_ReturnSameBoard_OnSubsequentCalls()
    {
        var userId = Guid.NewGuid();

        var firstCall = await F1BingoBoardsService.GetOrCreateBingoBoard(userId, EmptySeason);
        var secondCall = await F1BingoBoardsService.GetOrCreateBingoBoard(userId, EmptySeason);

        secondCall.Should().BeEquivalentTo(firstCall);
    }

    [Test]
    public async Task GetOrCreateBingoBoard_Should_ContainAllCardsForSeason()
    {
        var userId = Guid.NewGuid();

        var cardId1 = await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = CardsSeason,
                Description = "Board card 1",
                Probability = F1BingoCardProbability.Low,
                TotalRepeats = 1,
            }
        );
        var cardId2 = await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = CardsSeason,
                Description = "Board card 2",
                Probability = F1BingoCardProbability.Medium,
                TotalRepeats = 2,
            }
        );
        var cardId3 = await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = CardsSeason,
                Description = "Board card 3",
                Probability = F1BingoCardProbability.High,
                TotalRepeats = 3,
            }
        );

        var board = await F1BingoBoardsService.GetOrCreateBingoBoard(userId, CardsSeason);

        board.Should().Contain(cardId1);
        board.Should().Contain(cardId2);
        board.Should().Contain(cardId3);
    }

    [Test]
    public async Task GetOrCreateBingoBoard_Should_CreateSeparateBoardsForDifferentUsers()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = CardsSeason,
                Description = "Multi-user board card",
                Probability = F1BingoCardProbability.Medium,
                TotalRepeats = 1,
            }
        );

        var board1 = await F1BingoBoardsService.GetOrCreateBingoBoard(userId1, CardsSeason);
        var board2 = await F1BingoBoardsService.GetOrCreateBingoBoard(userId2, CardsSeason);

        board1.Should().NotBeNull();
        board2.Should().NotBeNull();
    }

    [Test]
    public async Task GetOrCreateBingoBoard_Should_CreateSeparateBoardsForDifferentSeasons()
    {
        var userId = Guid.NewGuid();

        var boardSeason1 = await F1BingoBoardsService.GetOrCreateBingoBoard(userId, EmptySeason);
        var boardSeason2 = await F1BingoBoardsService.GetOrCreateBingoBoard(userId, CardsSeason);

        boardSeason1.Should().NotBeNull();
        boardSeason2.Should().NotBeNull();
    }

    private const int EmptySeason = 9981;
    private const int CardsSeason = 9982;
}