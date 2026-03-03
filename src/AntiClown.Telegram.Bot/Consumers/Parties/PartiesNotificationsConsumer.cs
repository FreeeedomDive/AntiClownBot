using AntiClown.Messages.Dto.Parties;
using AntiClown.Telegram.Bot.Interactivity.Parties;
using MassTransit;

namespace AntiClown.Telegram.Bot.Consumers.Parties;

public class PartiesNotificationsConsumer : IConsumer<PartyUpdatedMessageDto>
{
    public PartiesNotificationsConsumer(
        IPartiesService partiesService,
        ILogger<PartiesNotificationsConsumer> log
    )
    {
        this.partiesService = partiesService;
        this.log = log;
    }

    public async Task Consume(ConsumeContext<PartyUpdatedMessageDto> context)
    {
        try
        {
            log.LogInformation("{ConsumerName} received message with party {PartyId}", nameof(PartiesNotificationsConsumer), context.Message.PartyId);
            await partiesService.CreateOrUpdateMessageAsync(context.Message.PartyId);
        }
        catch (Exception e)
        {
            log.LogError(e, "Unhandled exception in consumer {ConsumerName}", nameof(PartiesNotificationsConsumer));
        }
    }

    private readonly IPartiesService partiesService;
    private readonly ILogger<PartiesNotificationsConsumer> log;
}