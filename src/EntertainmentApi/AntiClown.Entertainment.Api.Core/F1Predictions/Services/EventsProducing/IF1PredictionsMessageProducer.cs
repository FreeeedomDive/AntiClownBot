namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;

public interface IF1PredictionsMessageProducer
{
    Task ProduceBingoCompletedAsync(Guid userId);
}
