﻿using AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;
using AntiClown.Entertainment.Api.Dto.DailyEvents.ResetsAndPayments;
using AntiClown.Messages.Dto.Events.Daily;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Daily;

public class DailyEventsDistributor : IConsumer<DailyEventMessageDto>
{
    public DailyEventsDistributor(
        IDailyEventConsumer<AnnounceEventDto> announceEventsConsumer,
        IDailyEventConsumer<ResetsAndPaymentsEventDto> resetsAndPaymentsEventDto,
        ILogger<DailyEventsDistributor> logger
    )
    {
        this.announceEventsConsumer = announceEventsConsumer;
        this.resetsAndPaymentsEventDto = resetsAndPaymentsEventDto;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<DailyEventMessageDto> context)
    {
        try
        {
            switch (context.Message.EventType)
            {
                case DailyEventTypeDto.Announce:
                    await announceEventsConsumer.ConsumeAsync(context);
                    break;
                case DailyEventTypeDto.PaymentsAndResets:
                    await resetsAndPaymentsEventDto.ConsumeAsync(context);
                    break;
                default:
                    logger.LogWarning(
                        "Found an unknown event {eventType} with id {eventId} in {ConsumerName}",
                        context.Message.EventType,
                        context.Message.EventId,
                        nameof(DailyEventsDistributor)
                    );
                    break;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception in consumer {ConsumerName}", nameof(DailyEventsDistributor));
        }
    }

    private readonly IDailyEventConsumer<AnnounceEventDto> announceEventsConsumer;
    private readonly ILogger<DailyEventsDistributor> logger;
    private readonly IDailyEventConsumer<ResetsAndPaymentsEventDto> resetsAndPaymentsEventDto;
}