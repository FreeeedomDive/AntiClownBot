using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Transfusion;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class TransfusionEventConsumer : ICommonEventConsumer<TransfusionEventDto>
{
    public TransfusionEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ITransfusionEmbedBuilder transfusionEmbedBuilder,
        IEmotesCache emotesCache,
        IUsersCache usersCache,
        IOptions<DiscordOptions> discordOptions,
        ILogger<TransfusionEventConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.transfusionEmbedBuilder = transfusionEmbedBuilder;
        this.emotesCache = emotesCache;
        this.usersCache = usersCache;
        this.discordOptions = discordOptions;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        logger.LogInformation("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);

        var transfusionEvent = await antiClownEntertainmentApiClient.CommonEvents.Transfusion.ReadAsync(eventId);
        var embed = await transfusionEmbedBuilder.BuildAsync(transfusionEvent);
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, embed);
    }

    private static string ConsumerName => nameof(TransfusionEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly ITransfusionEmbedBuilder transfusionEmbedBuilder;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IEmotesCache emotesCache;
    private readonly ILogger<TransfusionEventConsumer> logger;
    private readonly IUsersCache usersCache;
}