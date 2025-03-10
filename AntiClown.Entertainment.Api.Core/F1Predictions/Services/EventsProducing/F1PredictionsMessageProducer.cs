using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;

public class F1PredictionsMessageProducer(IBus bus) : IF1PredictionsMessageProducer
{
    public async Task ProducePredictionStartedAsync(Guid raceId, string name)
    {
        await bus.Publish(
            new F1PredictionStartedMessageDto
            {
                RaceId = raceId,
                Name = name,
            }
        );
    }

    public async Task ProducePredictionUpdatedAsync(Guid userId, Guid raceId, bool isNew)
    {
        await bus.Publish(new F1UserPredictionUpdatedMessageDto
        {
            UserId = userId,
            RaceId = raceId,
            IsNew = isNew,
        });
    }

    public async Task ProduceRaceResultUpdatedAsync(Guid raceId)
    {
        await bus.Publish(new F1RaceResultUpdatedMessageDto()
        {
            RaceId = raceId,
        });
    }

    public async Task ProduceRaceFinishedAsync(Guid raceId)
    {
        await bus.Publish(new F1RaceFinishedMessageDto
        {
            RaceId = raceId,
        });
    }
}