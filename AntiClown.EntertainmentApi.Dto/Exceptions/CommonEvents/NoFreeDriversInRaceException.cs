using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;

public class NoFreeDriversInRaceException : AntiClownConflictException
{
    public NoFreeDriversInRaceException(Guid raceId) : base($"No free drivers left in race {raceId}")
    {
        RaceId = raceId;
    }

    public Guid RaceId { get; }
}