using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Newtonsoft.Json;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class TransfusionEventConsumer : ICommonEventConsumer<TransfusionEventDto>
{
    public TransfusionEventConsumer(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<TransfusionEventConsumer> logger
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        var @event = await antiClownEntertainmentApiClient.CommonEvents.Transfusion.ReadAsync(eventId);
        logger.LogInformation(
            "{ConsumerName} received event with id {eventId}\n{eventInfo}",
            ConsumerName,
            eventId,
            JsonConvert.SerializeObject(@event, Formatting.Indented)
        );
    }

    private static string ConsumerName => nameof(TransfusionEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ILogger<TransfusionEventConsumer> logger;
}