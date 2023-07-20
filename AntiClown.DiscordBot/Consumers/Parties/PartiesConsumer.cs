using AntiClown.DiscordBot.Interactivity.Services.Parties;
using AntiClown.Messages.Dto.Parties;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Parties;

public class PartiesConsumer : IConsumer<PartyUpdatedMessageDto>
{
    public PartiesConsumer(
        IPartiesService partiesService,
        ILogger<PartiesConsumer> logger
    )
    {
        this.partiesService = partiesService;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<PartyUpdatedMessageDto> context)
    {
        logger.LogInformation("{ConsumerName} received message with party {PartyId}", nameof(PartiesConsumer), context.Message.PartyId);
        await partiesService.CreateOrUpdateAsync(context.Message.PartyId);
    }

    private readonly ILogger<PartiesConsumer> logger;
    private readonly IPartiesService partiesService;
}