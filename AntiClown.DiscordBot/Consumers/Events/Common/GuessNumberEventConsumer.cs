using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Newtonsoft.Json;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class GuessNumberEventConsumer : ICommonEventConsumer<GuessNumberEventDto>
{
    public GuessNumberEventConsumer(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<GuessNumberEventConsumer> logger
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

        var @event = await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.ReadAsync(eventId);
        logger.LogInformation(
            "{ConsumerName} received event with id {eventId}\n{eventInfo}",
            ConsumerName,
            eventId,
            JsonConvert.SerializeObject(@event, Formatting.Indented)
        );
    }

    public string ConsumerName => nameof(GuessNumberEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ILogger<GuessNumberEventConsumer> logger;
}