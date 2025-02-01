namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;

public interface IF1PredictionsMessageProducer
{
    Task ProducePredictionStartedAsync(Guid raceId, string name);
    Task ProducePredictionUpdatedAsync(Guid userId, Guid raceId, bool isNew);
    Task ProduceRaceResultUpdatedAsync(Guid raceId);
    Task ProduceRaceFinishedAsync(Guid raceId);
}