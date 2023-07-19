using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.RemoveCoolDowns;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class RemoveCoolDownsEventConsumer : ICommonEventConsumer<RemoveCoolDownsEventDto>
{
    public RemoveCoolDownsEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IRemoveCoolDownsEmbedBuilder removeCoolDownsEmbedBuilder,
        IOptions<DiscordOptions> discordOptions,
        ILogger<RemoveCoolDownsEventConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.removeCoolDownsEmbedBuilder = removeCoolDownsEmbedBuilder;
        this.discordOptions = discordOptions;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        var @event = await antiClownEntertainmentApiClient.CommonEvents.RemoveCoolDowns.ReadAsync(eventId);
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, await removeCoolDownsEmbedBuilder.BuildAsync(@event));
        logger.LogInformation("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(RemoveCoolDownsEventConsumer);
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly ILogger<RemoveCoolDownsEventConsumer> logger;
    private readonly IRemoveCoolDownsEmbedBuilder removeCoolDownsEmbedBuilder;
}