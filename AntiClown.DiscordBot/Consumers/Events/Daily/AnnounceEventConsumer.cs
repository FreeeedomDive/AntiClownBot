using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;
using AntiClown.Messages.Dto.Events.Daily;
using MassTransit;
using Newtonsoft.Json;

namespace AntiClown.DiscordBot.Consumers.Events.Daily;

public class AnnounceEventConsumer : IDailyEventConsumer<AnnounceEventDto>
{
    public AnnounceEventConsumer(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<AnnounceEventConsumer> logger
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<DailyEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        var @event = await antiClownEntertainmentApiClient.DailyEvents.Announce.ReadAsync(eventId);
        logger.LogInformation(
            "{ConsumerName} received event with id {eventId}\n{eventInfo}",
            ConsumerName,
            eventId,
            JsonConvert.SerializeObject(@event, Formatting.Indented)
        );
    }

    private static string ConsumerName => nameof(AnnounceEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ILogger<AnnounceEventConsumer> logger;
}