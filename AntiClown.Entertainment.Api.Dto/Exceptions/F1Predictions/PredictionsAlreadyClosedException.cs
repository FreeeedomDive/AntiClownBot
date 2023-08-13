using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;

public class PredictionsAlreadyClosedException : AntiClownConflictException
{
    public PredictionsAlreadyClosedException(Guid raceId) : base($"Predictions are already closed for race {raceId}")
    {
        RaceId = raceId;
    }

    public Guid RaceId { get; }
}