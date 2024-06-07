using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class RemoveCoolDownsEventConsumer : ICommonEventConsumer<RemoveCoolDownsEventDto>
{
    public RemoveCoolDownsEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IRemoveCoolDownsEmbedBuilder removeCoolDownsEmbedBuilder,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILoggerClient logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.removeCoolDownsEmbedBuilder = removeCoolDownsEmbedBuilder;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        var @event = await antiClownEntertainmentApiClient.RemoveCoolDownsEvent.ReadAsync(eventId);
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        await discordClientWrapper.Messages.SendAsync(botChannelId, await removeCoolDownsEmbedBuilder.BuildAsync(@event));

        await logger.InfoAsync("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(RemoveCoolDownsEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly ILoggerClient logger;
    private readonly IRemoveCoolDownsEmbedBuilder removeCoolDownsEmbedBuilder;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}