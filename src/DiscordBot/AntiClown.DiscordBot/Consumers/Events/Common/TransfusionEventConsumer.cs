using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Transfusion;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class TransfusionEventConsumer : ICommonEventConsumer<TransfusionEventDto>
{
    public TransfusionEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ITransfusionEmbedBuilder transfusionEmbedBuilder,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<TransfusionEventConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.transfusionEmbedBuilder = transfusionEmbedBuilder;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        var transfusionEvent = await antiClownEntertainmentApiClient.TransfusionEvent.ReadAsync(eventId);
        var embed = await transfusionEmbedBuilder.BuildAsync(transfusionEvent);
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        await discordClientWrapper.Messages.SendAsync(botChannelId, embed);

        logger.LogInformation("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(TransfusionEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly ILogger<TransfusionEventConsumer> logger;
    private readonly ITransfusionEmbedBuilder transfusionEmbedBuilder;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}