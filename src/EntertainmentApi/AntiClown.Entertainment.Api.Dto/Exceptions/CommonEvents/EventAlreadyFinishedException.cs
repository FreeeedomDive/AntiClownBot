using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;

public class EventAlreadyFinishedException : ConflictException
{
    public EventAlreadyFinishedException(Guid eventId) : base($"Event {eventId} is already finished")
    {
        EventId = eventId;
    }

    public Guid EventId { get; }
}