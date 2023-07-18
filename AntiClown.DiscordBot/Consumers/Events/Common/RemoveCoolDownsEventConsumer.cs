using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class RemoveCoolDownsEventConsumer : ICommonEventConsumer<RemoveCoolDownsEventDto>
{
    public RemoveCoolDownsEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IEmotesCache emotesCache,
        IOptions<DiscordOptions> discordOptions,
        ILogger<RemoveCoolDownsEventConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.emotesCache = emotesCache;
        this.discordOptions = discordOptions;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var messageContent = $"У меня хорошее настроение, несите ваши подношения {await emotesCache.GetEmoteAsTextAsync("peepoClap")}";
        var eventId = context.Message.EventId;
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, messageContent);
        logger.LogInformation("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(RemoveCoolDownsEventConsumer);

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IEmotesCache emotesCache;
    private readonly ILogger<RemoveCoolDownsEventConsumer> logger;
}