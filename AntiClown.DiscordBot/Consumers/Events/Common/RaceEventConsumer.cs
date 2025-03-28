﻿using AntiClown.DiscordBot.Interactivity.Services.Race;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class RaceEventConsumer : ICommonEventConsumer<RaceEventDto>
{
    public RaceEventConsumer(
        IRaceService raceService,
        ILogger<RaceEventConsumer> logger
    )
    {
        this.raceService = raceService;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        if (context.Message.Finished)
        {
#pragma warning disable CS4014 // Не дожидаемся окончания отображения всей гонки, выполняем в отдельном потоке
            Task.Run(
                async () =>
                {
                    try
                    {
                        await raceService.FinishAsync(eventId);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Failed to finish race {raceId}", eventId);
                    }
                }
            );
#pragma warning restore CS4014

            logger.LogInformation(
                "{ConsumerName} received FINISHED event with id {eventId}",
                ConsumerName,
                eventId
            );
            return;
        }

        await raceService.StartAsync(eventId);

        logger.LogInformation(
            "{ConsumerName} received event with id {eventId}",
            ConsumerName,
            eventId
        );
    }

    private static string ConsumerName => nameof(RaceEventConsumer);

    private readonly ILogger<RaceEventConsumer> logger;
    private readonly IRaceService raceService;
}