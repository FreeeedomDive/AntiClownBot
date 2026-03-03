using AntiClown.Entertainment.Api.Client.F1Bingo;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo;

namespace AntiClown.Entertainment.Api.Client.Extensions;

public static class F1BingoClientExtensions
{
    public static Task CreateBingoCard(
        this IF1BingoClient client,
        int season,
        string description,
        F1BingoCardProbabilityDto probability,
        int totalRepeats = 1,
        string? explanation = null
    )
    {
        return client.CreateCardAsync(
            new CreateF1BingoCardDto
            {
                Season = season,
                Description = description,
                Probability = probability,
                TotalRepeats = totalRepeats,
                Explanation = explanation,
            }
        );
    }
}