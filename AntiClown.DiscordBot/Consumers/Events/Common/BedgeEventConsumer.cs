using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Bedge;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class BedgeEventConsumer : ICommonEventConsumer<BedgeEventDto>
{
    public BedgeEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IEmotesCache emotesCache,
        IOptions<DiscordOptions> discordOptions,
        ILogger<BedgeEventConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.emotesCache = emotesCache;
        this.discordOptions = discordOptions;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var bedge = await emotesCache.GetEmoteAsTextAsync("Bedge");
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, bedge);

        logger.LogInformation("{ConsumerName} received bedge event with id {eventId}",ConsumerName,context.Message.EventId);
    }

    private static string ConsumerName => nameof(BedgeEventConsumer);

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IEmotesCache emotesCache;
    private readonly ILogger<BedgeEventConsumer> logger;
}