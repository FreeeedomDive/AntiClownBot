using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Messages.Dto.Events.Daily;
using AutoMapper;
using MassTransit;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Services.Messages;

public class DailyEventsMessageProducer : IDailyEventsMessageProducer
{
    public DailyEventsMessageProducer(IBus bus, IMapper mapper)
    {
        this.bus = bus;
        this.mapper = mapper;
    }

    public async Task ProduceAsync(DailyEventBase dailyEvent)
    {
        var eventMessage = mapper.Map<DailyEventMessageDto>(dailyEvent);
        await bus.Publish(eventMessage);
    }

    private readonly IBus bus;
    private readonly IMapper mapper;
}