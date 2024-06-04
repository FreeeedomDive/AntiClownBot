using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;
using AntiClown.Messages.Dto.Events.Daily;
using DSharpPlus.Entities;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Events.Daily;

public class AnnounceEventConsumer : IDailyEventConsumer<AnnounceEventDto>
{
    public AnnounceEventConsumer(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownDataApiClient antiClownDataApiClient,
        IUsersCache usersCache,
        ILoggerClient logger
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.usersCache = usersCache;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<DailyEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        await logger.InfoAsync(
            "{ConsumerName} received event with id {eventId}",
            ConsumerName,
            eventId
        );
        var @event = await antiClownEntertainmentApiClient.AnnounceEvent.ReadAsync(eventId);
        var totalEarnings = @event.Earnings.Values.Sum();
        var embedBuilder = new DiscordEmbedBuilder()
                           .WithTitle("Подошел к концу очередной день в Clown-City")
                           .WithColor(DiscordColor.Chartreuse);
        if (@event.Earnings.Count != 0)
        {
            var max = @event.Earnings.MaxBy(kv => kv.Value);
            var major = await usersCache.GetMemberByApiIdAsync(max.Key);
            var min = @event.Earnings.MinBy(kv => kv.Value);
            var minor = await usersCache.GetMemberByApiIdAsync(min.Key);
            embedBuilder.AddField(
                $"Граждане заработали за день {totalEarnings.ToPluralizedString("скам-койн", "скам-койна", "скам-койнов")}",
                $"Мажор дня - {major.ServerOrUserName()}, {max.Value.ToPluralizedString("скам-койн", "скам-койна", "скам-койнов")}\n"
                + $"Бомж дня - {minor.ServerOrUserName()}, {min.Value.ToPluralizedString("скам-койн", "скам-койна", "скам-койнов")}\n"
            );
        }
        else
        {
            embedBuilder.AddField(
                "Никто ничего не заработал",
                "Такие дела"
            );
        }

        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        await discordClientWrapper.Messages.SendAsync(botChannelId, embedBuilder.Build());
    }

    private static string ConsumerName => nameof(AnnounceEventConsumer);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly ILoggerClient logger;
    private readonly IUsersCache usersCache;
}