using AntiClown.Messages.Dto.Parties;
using AntiClown.Telegram.Bot.Interactivity.Parties;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.Telegram.Bot.Consumers.Parties;

public class PartiesNotificationsConsumer : IConsumer<PartyUpdatedMessageDto>
{
    public PartiesNotificationsConsumer(
        IPartiesService partiesService,
        ILoggerClient logger
    )
    {
        this.partiesService = partiesService;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<PartyUpdatedMessageDto> context)
    {
        try
        {
            await logger.InfoAsync("{ConsumerName} received message with party {PartyId}", nameof(PartiesNotificationsConsumer), context.Message.PartyId);
            await partiesService.CreateOrUpdateMessageAsync(context.Message.PartyId);
        }
        catch (Exception e)
        {
            await logger.ErrorAsync(e, "Unhandled exception in consumer {ConsumerName}", nameof(PartiesNotificationsConsumer));
        }
    }

    private readonly IPartiesService partiesService;
    private readonly ILoggerClient logger;
}