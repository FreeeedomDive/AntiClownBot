using AntiClown.DiscordBot.Interactivity.Services.Lottery;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class LotteryEventConsumer : ICommonEventConsumer<LotteryEventDto>
{
    public LotteryEventConsumer(
        ILotteryService lotteryService,
        ILogger<LotteryEventConsumer> logger
    )
    {
        this.lotteryService = lotteryService;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        if (context.Message.Finished)
        {
            await lotteryService.FinishAsync(eventId);

            logger.LogInformation(
                "{ConsumerName} received FINISHED event with id {eventId}",
                ConsumerName,
                eventId
            );
            return;
        }

        await lotteryService.StartAsync(eventId);

        logger.LogInformation("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(LotteryEventConsumer);
    private readonly ILogger<LotteryEventConsumer> logger;

    private readonly ILotteryService lotteryService;
}