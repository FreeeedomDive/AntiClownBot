using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Messages.Dto;
using AutoMapper;
using MassTransit;

namespace AntiClown.Api.Core.Economies.Services;

public class TributeMessageProducer : ITributeMessageProducer
{
    public TributeMessageProducer(IBus bus, IMapper mapper)
    {
        this.bus = bus;
        this.mapper = mapper;
    }

    public async Task ProduceAsync(Tribute tribute)
    {
        var message = mapper.Map<TributeMessageDto>(tribute);
        await bus.Publish(message);
    }

    private readonly IBus bus;
    private readonly IMapper mapper;
}