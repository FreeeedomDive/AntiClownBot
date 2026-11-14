using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;

public class F1PredictionsMessageProducer(IBus bus) : IF1PredictionsMessageProducer
{
    public async Task ProduceBingoCompletedAsync(Guid userId)
    {
        await bus.Publish(
            new F1BingoBoardCompletedMessageDto
            {
                UserId = userId,
            }
        );
    }
}
