﻿using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Tributes;
using AntiClown.Messages.Dto.Tributes;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Tributes;

public class TributesConsumer : IConsumer<TributeMessageDto>
{
    public TributesConsumer(
        IDiscordClientWrapper discordClientWrapper,
        ITributeEmbedBuilder tributeEmbedBuilder,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<TributesConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.tributeEmbedBuilder = tributeEmbedBuilder;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<TributeMessageDto> context)
    {
        try
        {
            var tribute = context.Message;
            logger.LogInformation("Received auto tribute for user {userId}", tribute.UserId);
            var tributeEmbed = await tributeEmbedBuilder.BuildForSuccessfulTributeAsync(tribute.Tribute);
            
            var tributeChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "TributeChannelId");
            await discordClientWrapper.Messages.SendAsync(tributeChannelId, tributeEmbed);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception in consumer {ConsumerName}", nameof(TributesConsumer));
        }
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly ILogger<TributesConsumer> logger;
    private readonly ITributeEmbedBuilder tributeEmbedBuilder;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}