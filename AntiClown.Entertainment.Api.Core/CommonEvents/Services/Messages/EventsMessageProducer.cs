using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Messages.Dto.Events.Common;
using AutoMapper;
using MassTransit;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;

public class EventsMessageProducer : IEventsMessageProducer
{
    public EventsMessageProducer(IBus bus, IMapper mapper)
    {
        this.bus = bus;
        this.mapper = mapper;
    }

    public async Task ProduceAsync(CommonEventBase eventBase)
    {
        var @event = mapper.Map<CommonEventMessageDto>(eventBase);
        await bus.Publish(@event);
    }

    private readonly IBus bus;
    private readonly IMapper mapper;
}