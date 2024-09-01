using AntiClown.Messages.Dto.Parties;
using MassTransit;

namespace AntiClown.Telegram.Bot.Consumers.Parties;

public class PartiesNotificationsConsumer : IConsumer<PartyUpdatedMessageDto>
{
    public PartiesNotificationsConsumer(ILogger<PartiesNotificationsConsumer> logger)
    {
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<PartyUpdatedMessageDto> context)
    {
        var partyId = context.Message.PartyId;
        logger.LogInformation("Received party {partyId}", partyId);
    }

    private readonly ILogger<PartiesNotificationsConsumer> logger;
}