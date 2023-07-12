using AntiClown.EntertainmentApi.Dto.CommonEvents.Bedge;
using AntiClown.EntertainmentApi.Dto.CommonEvents.GuessNumber;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Lottery;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;
using AntiClown.EntertainmentApi.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Transfusion;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

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
        ILogger<CommonEventsDistributor> logger
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
                logger.LogWarning(
                    "Found an unknown event {context.Message.EventType} with id {eventId} in {ConsumerName}",
                    context.Message.EventType,
                    context.Message.EventId,
                    nameof(CommonEventsDistributor)
                );
                break;
        }
    }

    private readonly ICommonEventConsumer<GuessNumberEventDto> guessNumberEventConsumer;
    private readonly ICommonEventConsumer<LotteryEventDto> lotteryEventConsumer;
    private readonly ICommonEventConsumer<RaceEventDto> raceEventConsumer;
    private readonly ICommonEventConsumer<RemoveCoolDownsEventDto> removeCoolDownsEventConsumer;
    private readonly ICommonEventConsumer<TransfusionEventDto> transfusionEventConsumer;
    private readonly ICommonEventConsumer<BedgeEventDto> bedgeEventConsumer;
    private readonly ILogger<CommonEventsDistributor> logger;
}