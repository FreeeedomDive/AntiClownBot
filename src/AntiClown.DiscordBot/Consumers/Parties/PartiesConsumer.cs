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
        try
        {
            logger.LogInformation("{ConsumerName} received message with party {PartyId}", nameof(PartiesConsumer), context.Message.PartyId);
            await partiesService.CreateOrUpdateAsync(context.Message.PartyId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception in consumer {ConsumerName}", nameof(PartiesConsumer));
        }
    }

    private readonly ILogger<PartiesConsumer> logger;
    private readonly IPartiesService partiesService;
}