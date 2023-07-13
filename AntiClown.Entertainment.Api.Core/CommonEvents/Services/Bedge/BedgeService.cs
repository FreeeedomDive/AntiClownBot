using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Bedge;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Bedge;

public class BedgeService : IBedgeService
{
    public BedgeService(ICommonEventsMessageProducer commonEventsMessageProducer)
    {
        this.commonEventsMessageProducer = commonEventsMessageProducer;
    }

    public async Task<BedgeEvent> ReadAsync(Guid eventId)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var @event = BedgeEvent.Create();
        await commonEventsMessageProducer.ProduceAsync(@event);

        return @event.Id;
    }

    private readonly ICommonEventsMessageProducer commonEventsMessageProducer;
}