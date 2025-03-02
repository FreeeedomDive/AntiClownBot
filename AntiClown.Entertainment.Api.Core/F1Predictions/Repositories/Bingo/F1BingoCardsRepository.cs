using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

public class F1BingoCardsRepository(
    ISqlRepository<F1BingoCardStorageElement> sqlRepository
) : IF1BingoCardsRepository
{
    public async Task<F1BingoCard[]> FindAsync(int season)
    {
        var result = await sqlRepository.FindAsync(x => x.Season == season);
        return result.Select(ToModel).ToArray();
    }

    public async Task CreateAsync(F1BingoCard bingoCard)
    {
        await sqlRepository.CreateAsync(ToStorageElement(bingoCard));
    }

    public async Task UpdateAsync(F1BingoCard bingoCard)
    {
        await sqlRepository.UpdateAsync(
            bingoCard.Id, x =>
            {
                x.CompletedRepeats = bingoCard.CompletedRepeats;
                x.IsCompleted = bingoCard.IsCompleted;
            }
        );
    }

    private static F1BingoCard ToModel(F1BingoCardStorageElement storageElement) => new()
    {
        Id = storageElement.Id,
        Season = storageElement.Season,
        Description = storageElement.Description,
        Explanation = storageElement.Explanation,
        Probability = Enum.Parse<F1BingoCardProbability>(storageElement.Probability),
        TotalRepeats = storageElement.TotalRepeats,
        CompletedRepeats = storageElement.CompletedRepeats,
        IsCompleted = storageElement.IsCompleted,
    };

    private static F1BingoCardStorageElement ToStorageElement(F1BingoCard model) => new()
    {
        Id = model.Id,
        Season = model.Season,
        Description = model.Description,
        Explanation = model.Explanation,
        Probability = model.Probability.ToString(),
        TotalRepeats = model.TotalRepeats,
        CompletedRepeats = model.CompletedRepeats,
        IsCompleted = model.IsCompleted,
    };
}