using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;

public class NoFreeDriversInRaceException : ConflictException
{
    public NoFreeDriversInRaceException(Guid raceId) : base($"No free drivers left in race {raceId}")
    {
        RaceId = raceId;
    }

    public Guid RaceId { get; }
}