using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions.Bingo;
using FluentAssertions;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

public class F1BingoCardsServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task CreateCardAsync_Should_CreateCard()
    {
        var createCard = new CreateF1BingoCard
        {
            Season = TestSeason,
            Description = "Test bingo card",
            Explanation = "Test explanation",
            Probability = F1BingoCardProbability.Medium,
            TotalRepeats = 3,
        };

        var cardId = await F1BingoCardsService.CreateCardAsync(createCard);

        var cards = await F1BingoCardsService.FindAsync(TestSeason);
        cards.Should().Contain(c =>
            c.Id == cardId
            && c.Description == createCard.Description
            && c.Season == TestSeason
            && c.TotalRepeats == createCard.TotalRepeats
            && c.CompletedRepeats == 0
            && !c.IsCompleted
        );
    }

    [Test]
    public async Task CreateCardAsync_Should_SetDefaultsCorrectly()
    {
        var cardId = await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = TestSeason,
                Description = "Defaults test card",
                Probability = F1BingoCardProbability.Low,
                TotalRepeats = 1,
            }
        );

        var cards = await F1BingoCardsService.FindAsync(TestSeason);
        var card = cards.Single(c => c.Id == cardId);

        card.CompletedRepeats.Should().Be(0);
        card.IsCompleted.Should().BeFalse();
        card.Explanation.Should().BeNull();
    }

    [Test]
    public async Task FindAsync_Should_ReturnOnlyCardsForRequestedSeason()
    {
        var cardId = await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = TestSeason,
                Description = "Season filter card",
                Probability = F1BingoCardProbability.High,
                TotalRepeats = 2,
            }
        );
        await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = OtherSeason,
                Description = "Other season card",
                Probability = F1BingoCardProbability.Low,
                TotalRepeats = 1,
            }
        );

        var cards = await F1BingoCardsService.FindAsync(TestSeason);

        cards.Should().Contain(c => c.Id == cardId);
        cards.Should().NotContain(c => c.Season == OtherSeason);
    }

    [Test]
    public async Task UpdateCardAsync_Should_UpdateCompletedRepeats()
    {
        var cardId = await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = TestSeason,
                Description = "Update repeats card",
                Probability = F1BingoCardProbability.Medium,
                TotalRepeats = 5,
            }
        );

        await F1BingoCardsService.UpdateCardAsync(cardId, new UpdateF1BingoCard { NewRepeatsCount = 3 });

        var cards = await F1BingoCardsService.FindAsync(TestSeason);
        var card = cards.Single(c => c.Id == cardId);
        card.CompletedRepeats.Should().Be(3);
        card.IsCompleted.Should().BeFalse();
    }

    [Test]
    public async Task UpdateCardAsync_Should_MarkAsCompleted_WhenReachesTotalRepeats()
    {
        var cardId = await F1BingoCardsService.CreateCardAsync(
            new CreateF1BingoCard
            {
                Season = TestSeason,
                Description = "Complete card",
                Probability = F1BingoCardProbability.High,
                TotalRepeats = 2,
            }
        );

        await F1BingoCardsService.UpdateCardAsync(cardId, new UpdateF1BingoCard { NewRepeatsCount = 2 });

        var cards = await F1BingoCardsService.FindAsync(TestSeason);
        var card = cards.Single(c => c.Id == cardId);
        card.IsCompleted.Should().BeTrue();
        card.CompletedRepeats.Should().Be(2);
    }

    [Test]
    public async Task UpdateCardAsync_Should_Throw_WhenCardNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        var act = () => F1BingoCardsService.UpdateCardAsync(nonExistentId, new UpdateF1BingoCard { NewRepeatsCount = 1 });

        await act.Should().ThrowAsync<F1BingoCardNotFoundException>();
    }

    private const int TestSeason = 9991;
    private const int OtherSeason = 9992;
}