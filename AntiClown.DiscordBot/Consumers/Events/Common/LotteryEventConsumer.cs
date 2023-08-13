using AntiClown.DiscordBot.Interactivity.Services.Lottery;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class LotteryEventConsumer : ICommonEventConsumer<LotteryEventDto>
{
    public LotteryEventConsumer(
        ILotteryService lotteryService,
        ILoggerClient logger
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

            await logger.InfoAsync(
                "{ConsumerName} received FINISHED event with id {eventId}",
                ConsumerName,
                eventId
            );
            return;
        }

        await lotteryService.StartAsync(eventId);

        await logger.InfoAsync("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(LotteryEventConsumer);
    private readonly ILoggerClient logger;

    private readonly ILotteryService lotteryService;
}