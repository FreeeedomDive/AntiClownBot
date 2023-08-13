using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AntiClown.Messages.Dto.Parties;
using MassTransit;

namespace AntiClown.Entertainment.Api.Core.Parties.Services.Messages;

public class PartiesMessageProducer : IPartiesMessageProducer
{
    public PartiesMessageProducer(IBus bus)
    {
        this.bus = bus;
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
}