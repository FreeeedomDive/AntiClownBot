using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AntiClown.Messages.Dto.Parties;
using AutoMapper;
using MassTransit;

namespace AntiClown.Entertainment.Api.Core.Parties.Services.Messages;

public class PartiesMessageProducer : IPartiesMessageProducer
{
    public PartiesMessageProducer(
        IBus bus,
        IMapper mapper
    )
    {
        this.bus = bus;
        this.mapper = mapper;
    }

    public async Task ProduceAsync(Party party)
    {
        await bus.Publish(
            new PartyUpdatedMessageDto
            {
                PartyId = party.Id,
            }
        );
    }

    private readonly IBus bus;
    private readonly IMapper mapper;
}