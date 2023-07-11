using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Bedge;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Bedge;

public class BedgeService : IBedgeService
{
    public BedgeService(IEventsMessageProducer eventsMessageProducer)
    {
        this.eventsMessageProducer = eventsMessageProducer;
    }

    public async Task<BedgeEvent> ReadAsync(Guid eventId)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var @event = BedgeEvent.Create();
        await eventsMessageProducer.ProduceAsync(@event);

        return @event.Id;
    }

    private readonly IEventsMessageProducer eventsMessageProducer;
}