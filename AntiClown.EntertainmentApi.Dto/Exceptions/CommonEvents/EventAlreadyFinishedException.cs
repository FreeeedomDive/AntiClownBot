using AntiClown.Core.Dto.Exceptions;

namespace AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;

public class EventAlreadyFinishedException : AntiClownConflictException
{
    public EventAlreadyFinishedException(Guid eventId) : base($"Event {eventId} is already finished")
    {
        EventId = eventId;
    }
    
    public Guid EventId { get; }
}