using AntiClown.Messages.Dto.Parties;
using AntiClown.Telegram.Bot.Interactivity.Parties;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.Telegram.Bot.Consumers.Parties;

public class PartiesNotificationsConsumer : IConsumer<PartyUpdatedMessageDto>
{
    public PartiesNotificationsConsumer(
        IPartiesService partiesService,
        ILogger<PartiesNotificationsConsumer> log,
        ILoggerClient logger
    )
    {
        this.partiesService = partiesService;
        this.log = log;
        this.logger = logger;
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
    private readonly ILoggerClient logger;
}