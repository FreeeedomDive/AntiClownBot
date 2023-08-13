using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Transfusion;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Microsoft.Extensions.Options;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class TransfusionEventConsumer : ICommonEventConsumer<TransfusionEventDto>
{
    public TransfusionEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ITransfusionEmbedBuilder transfusionEmbedBuilder,
        IOptions<DiscordOptions> discordOptions,
        ILoggerClient logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.transfusionEmbedBuilder = transfusionEmbedBuilder;
        this.discordOptions = discordOptions;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        var transfusionEvent = await antiClownEntertainmentApiClient.CommonEvents.Transfusion.ReadAsync(eventId);
        var embed = await transfusionEmbedBuilder.BuildAsync(transfusionEvent);
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, embed);

        await logger.InfoAsync("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(TransfusionEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly ILoggerClient logger;
    private readonly ITransfusionEmbedBuilder transfusionEmbedBuilder;
}