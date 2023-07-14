using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Newtonsoft.Json;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class LotteryEventConsumer : ICommonEventConsumer<LotteryEventDto>
{
    public LotteryEventConsumer(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<LotteryEventConsumer> logger
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

        var @event = await antiClownEntertainmentApiClient.CommonEvents.Lottery.ReadAsync(eventId);
        logger.LogInformation(
            "{ConsumerName} received event with id {eventId}\n{eventInfo}",
            ConsumerName,
            eventId,
            JsonConvert.SerializeObject(@event, Formatting.Indented)
        );
    }

    private static string ConsumerName => nameof(LotteryEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ILogger<LotteryEventConsumer> logger;
}