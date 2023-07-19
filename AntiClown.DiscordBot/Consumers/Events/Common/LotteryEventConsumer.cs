using AntiClown.DiscordBot.Interactivity.Services.Lottery;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class LotteryEventConsumer : ICommonEventConsumer<LotteryEventDto>
{
    public LotteryEventConsumer(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILotteryService lotteryService,
        ILogger<LotteryEventConsumer> logger
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.lotteryService = lotteryService;
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
            await lotteryService.FinishAsync(eventId);
            return;
        }

        await lotteryService.StartAsync(eventId);
        logger.LogInformation("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(LotteryEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ILogger<LotteryEventConsumer> logger;
    private readonly ILotteryService lotteryService;
}