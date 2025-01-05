using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Bedge;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class BedgeEventConsumer : ICommonEventConsumer<BedgeEventDto>
{
    public BedgeEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IEmotesCache emotesCache,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<BedgeEventConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.emotesCache = emotesCache;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var bedge = await emotesCache.GetEmoteAsTextAsync("Bedge");
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        await discordClientWrapper.Messages.SendAsync(botChannelId, bedge);

        logger.LogInformation("{ConsumerName} received bedge event with id {eventId}",ConsumerName,context.Message.EventId);
    }

    private static string ConsumerName => nameof(BedgeEventConsumer);

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesCache emotesCache;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly ILogger<BedgeEventConsumer> logger;
}