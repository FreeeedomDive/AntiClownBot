namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public interface IF1PredictionsMessageProducer
{
    Task ProducePredictionUpdatedAsync(Guid userId, Guid raceId, bool isNew);
    Task ProduceRaceResultUpdatedAsync(Guid raceId);
    Task ProduceRaceFinishedAsync(Guid raceId);
}