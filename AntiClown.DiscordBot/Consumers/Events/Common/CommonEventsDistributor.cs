using AntiClown.Entertainment.Api.Dto.CommonEvents.Bedge;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class CommonEventsDistributor : IConsumer<CommonEventMessageDto>
{
    public CommonEventsDistributor(
        ICommonEventConsumer<GuessNumberEventDto> guessNumberEventConsumer,
        ICommonEventConsumer<LotteryEventDto> lotteryEventConsumer,
        ICommonEventConsumer<RaceEventDto> raceEventConsumer,
        ICommonEventConsumer<RemoveCoolDownsEventDto> removeCoolDownsEventConsumer,
        ICommonEventConsumer<TransfusionEventDto> transfusionEventConsumer,
        ICommonEventConsumer<BedgeEventDto> bedgeEventConsumer,
        ILoggerClient logger
    )
    {
        this.guessNumberEventConsumer = guessNumberEventConsumer;
        this.lotteryEventConsumer = lotteryEventConsumer;
        this.raceEventConsumer = raceEventConsumer;
        this.removeCoolDownsEventConsumer = removeCoolDownsEventConsumer;
        this.transfusionEventConsumer = transfusionEventConsumer;
        this.bedgeEventConsumer = bedgeEventConsumer;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<CommonEventMessageDto> context)
    {
        try
        {
            switch (context.Message.EventType)
            {
                case CommonEventTypeDto.GuessNumber:
                    await guessNumberEventConsumer.ConsumeAsync(context);
                    break;
                case CommonEventTypeDto.Lottery:
                    await lotteryEventConsumer.ConsumeAsync(context);
                    break;
                case CommonEventTypeDto.Race:
                    await raceEventConsumer.ConsumeAsync(context);
                    break;
                case CommonEventTypeDto.RemoveCooldowns:
                    await removeCoolDownsEventConsumer.ConsumeAsync(context);
                    break;
                case CommonEventTypeDto.Transfusion:
                    await transfusionEventConsumer.ConsumeAsync(context);
                    break;
                case CommonEventTypeDto.Bedge:
                    await bedgeEventConsumer.ConsumeAsync(context);
                    break;
                default:
                    await logger.WarnAsync(
                        "Found an unknown event {eventType} with id {eventId} in {ConsumerName}",
                        context.Message.EventType,
                        context.Message.EventId,
                        nameof(CommonEventsDistributor)
                    );
                    break;
            }
        }
        catch (Exception e)
        {
            await logger.ErrorAsync(e, "Unhandled exception in consumer {ConsumerName}", nameof(CommonEventsDistributor));
        }
    }

    private readonly ICommonEventConsumer<BedgeEventDto> bedgeEventConsumer;
    private readonly ICommonEventConsumer<GuessNumberEventDto> guessNumberEventConsumer;
    private readonly ILoggerClient logger;
    private readonly ICommonEventConsumer<LotteryEventDto> lotteryEventConsumer;
    private readonly ICommonEventConsumer<RaceEventDto> raceEventConsumer;
    private readonly ICommonEventConsumer<RemoveCoolDownsEventDto> removeCoolDownsEventConsumer;
    private readonly ICommonEventConsumer<TransfusionEventDto> transfusionEventConsumer;
}