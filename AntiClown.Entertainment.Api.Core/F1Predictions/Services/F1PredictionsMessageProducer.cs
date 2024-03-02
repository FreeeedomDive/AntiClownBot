using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public class F1PredictionsMessageProducer : IF1PredictionsMessageProducer
{
    public F1PredictionsMessageProducer(IBus bus)
    {
        this.bus = bus;
    }

    public async Task ProducePredictionUpdatedAsync(Guid userId, Guid raceId)
    {
        await bus.Publish(new F1UserPredictionUpdatedMessageDto
        {
            UserId = userId,
            RaceId = raceId,
        });
    }

    private readonly IBus bus;
}