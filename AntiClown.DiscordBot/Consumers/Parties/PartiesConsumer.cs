﻿using AntiClown.DiscordBot.Interactivity.Services.Parties;
using AntiClown.Messages.Dto.Parties;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Parties;

public class PartiesConsumer : IConsumer<PartyUpdatedMessageDto>
{
    public PartiesConsumer(
        IPartiesService partiesService,
        ILoggerClient logger
    )
    {
        this.partiesService = partiesService;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<PartyUpdatedMessageDto> context)
    {
        try
        {
            await logger.InfoAsync("{ConsumerName} received message with party {PartyId}", nameof(PartiesConsumer), context.Message.PartyId);
            await partiesService.CreateOrUpdateAsync(context.Message.PartyId);
        }
        catch (Exception e)
        {
            await logger.ErrorAsync(e, "Unhandled exception in consumer {ConsumerName}", nameof(PartiesConsumer));
        }
    }

    private readonly ILoggerClient logger;
    private readonly IPartiesService partiesService;
}