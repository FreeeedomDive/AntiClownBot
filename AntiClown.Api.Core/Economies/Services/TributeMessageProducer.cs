using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Dto.Economies;
using AntiClown.Messages.Dto.Tributes;
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
        var dto = mapper.Map<TributeDto>(tribute);
        await bus.Publish(new TributeMessageDto
        {
            UserId = tribute.UserId,
            Tribute = dto,
        });
    }

    private readonly IBus bus;
    private readonly IMapper mapper;
}