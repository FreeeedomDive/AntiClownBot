using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Newtonsoft.Json;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class RaceEventConsumer : ICommonEventConsumer<RaceEventDto>
{
    public RaceEventConsumer(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<RaceEventConsumer> logger
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        if (context.Message.Finished)
        {
            logger.LogInformation(
                "{ConsumerName} received FINISHED event with id {eventId}",
                ConsumerName,
                eventId
            );
            return;
        }

        var @event = await antiClownEntertainmentApiClient.CommonEvents.Race.ReadAsync(eventId);
        logger.LogInformation(
            "{ConsumerName} received event with id {eventId}\n{eventInfo}",
            ConsumerName,
            eventId,
            JsonConvert.SerializeObject(@event, Formatting.Indented)
        );
    }

    private static string ConsumerName => nameof(RaceEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ILogger<RaceEventConsumer> logger;
}