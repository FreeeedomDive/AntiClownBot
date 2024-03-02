namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public interface IF1PredictionsMessageProducer
{
    Task ProducePredictionUpdatedAsync(Guid userId, Guid raceId);
}