using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public static class F1BingoCompletionsCalculator
{
    public static F1BingoBoard[] GetCompletedBoards(F1BingoBoard[] boards, F1BingoCard[] cards)
    {
        var bingoCompletions = new[]
        {
            // horizontal
            new[] { 0, 1, 2, 3, 4 },
            new[] { 5, 6, 7, 8, 9 },
            new[] { 10, 11, 12, 13, 14 },
            new[] { 15, 16, 17, 18, 19 },
            new[] { 20, 21, 22, 23, 24 },
            // vertical
            new[] { 0, 5, 10, 15, 20 },
            new[] { 1, 6, 11, 16, 21 },
            new[] { 2, 7, 12, 17, 22 },
            new[] { 3, 8, 13, 18, 23 },
            new[] { 4, 9, 14, 19, 24 },
            // diagonal
            new[] { 0, 6, 12, 18, 24 },
            new[] { 4, 8, 12, 16, 20 },
        };

        var boardsWithBingo = new List<F1BingoBoard>();
        var completedCards = cards.Where(x => x.IsCompleted).Select(x => x.Id).ToHashSet();
        foreach (var board in boards)
        {
            var userCompletedCardsIndices = board.Cards
                                                 .Select((x, i) => completedCards.Contains(x) ? i : (int?)null)
                                                 .Where(x => x is not null)
                                                 .Select(x => x!.Value)
                                                 .ToArray();
            var hasFullIntersects = bingoCompletions.Any(x => x.Intersect(userCompletedCardsIndices).Count() == 5);
            if (!hasFullIntersects)
            {
                continue;
            }

            boardsWithBingo.Add(board);
        }

        return boardsWithBingo.ToArray();
    }
}