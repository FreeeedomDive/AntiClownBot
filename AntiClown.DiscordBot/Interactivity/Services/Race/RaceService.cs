using System.Text;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Domain.Race;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.Interactivity.Services.Race;

public class RaceService : IRaceService
{
    public RaceService(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IDiscordClientWrapper discordClientWrapper,
        IEmotesCache emotesCache,
        IInteractivityRepository interactivityRepository,
        IAntiClownDataApiClient antiClownDataApiClient,
        IUsersCache usersCache
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
        this.interactivityRepository = interactivityRepository;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    public async Task StartAsync(Guid eventId)
    {
        var botDiscordId = await discordClientWrapper.Members.GetBotIdAsync();
        var botId = await usersCache.GetApiIdByMemberIdAsync(botDiscordId);
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        await antiClownEntertainmentApiClient.CommonEvents.Race.AddParticipantAsync(eventId, botId);
        var raceEvent = await antiClownEntertainmentApiClient.CommonEvents.Race.ReadAsync(eventId);
        var welcomeMessageBuilder = await BuildEventMessageAsync(raceEvent);
        var welcomeMessage = await discordClientWrapper.Messages.SendAsync(botChannelId, welcomeMessageBuilder);
        var gridMessageContent = await BuildStartingGridMessageAsync(raceEvent);
        var gridMessage = await discordClientWrapper.Messages.SendAsync(botChannelId, gridMessageContent);
        var interactivity = new Interactivity<RaceDetails>
        {
            Id = eventId,
            Type = InteractivityType.Race,
            AuthorId = 0,
            MessageId = welcomeMessage.Id,
            Details = new RaceDetails
            {
                MainRaceMessageId = gridMessage.Id,
            },
        };
        await interactivityRepository.CreateAsync(interactivity);
    }

    public async Task AddParticipantAsync(Guid eventId, ulong memberId)
    {
        var interactivity = await interactivityRepository.TryReadAsync<RaceDetails>(eventId);
        if (interactivity is null)
        {
            return;
        }

        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        var userId = await usersCache.GetApiIdByMemberIdAsync(memberId);
        await antiClownEntertainmentApiClient.CommonEvents.Race.AddParticipantAsync(eventId, userId);
        var raceEvent = await antiClownEntertainmentApiClient.CommonEvents.Race.ReadAsync(eventId);

        var welcomeMessageBuilder = await BuildEventMessageAsync(raceEvent);
        var welcomeMessage = await discordClientWrapper.Messages.FindMessageAsync(botChannelId, interactivity.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(welcomeMessage, welcomeMessageBuilder);

        var newGridMessageContent = await BuildStartingGridMessageAsync(raceEvent);
        var gridMessage = await discordClientWrapper.Messages.FindMessageAsync(botChannelId, interactivity.Details!.MainRaceMessageId);
        await discordClientWrapper.Messages.ModifyAsync(gridMessage, newGridMessageContent);
    }

    public async Task FinishAsync(Guid eventId)
    {
        var interactivity = await interactivityRepository.TryReadAsync<RaceDetails>(eventId);
        if (interactivity is null)
        {
            return;
        }

        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        var raceEvent = await antiClownEntertainmentApiClient.CommonEvents.Race.ReadAsync(eventId);

        var welcomeMessageBuilder = await BuildEventMessageAsync(raceEvent);
        var welcomeMessage = await discordClientWrapper.Messages.FindMessageAsync(botChannelId, interactivity.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(welcomeMessage, welcomeMessageBuilder);

        var gridMessage = await discordClientWrapper.Messages.FindMessageAsync(botChannelId, interactivity.Details!.MainRaceMessageId);
        var members = raceEvent
                      .Participants
                      .Where(x => x.UserId is not null)
                      .ToDictionary(x => x.UserId!.Value, x => usersCache.GetMemberByApiIdAsync(x.UserId!.Value));
        for (var i = 0; i < raceEvent.Sectors.Length; i++)
        {
            var currentSectorMessage = await BuildRaceSnapshotMessageAsync(members, raceEvent, i);
            await discordClientWrapper.Messages.ModifyAsync(gridMessage, currentSectorMessage);
            await Task.Delay(2 * 1000);
        }
    }

    private async Task<DiscordMessageBuilder> BuildEventMessageAsync(RaceEventDto raceEvent)
    {
        var ping = await antiClownDataApiClient.Settings.ReadBoolAsync(SettingsCategory.DiscordBot, "PingOnEvents") ? "@everyone" : "";
        var raceWelcomeEmbed = new DiscordEmbedBuilder()
                               .WithTitle("Гонка")
                               .WithColor(DiscordColor.Teal)
                               .AddField(
                                   "Начинаем гоночку!",
                                   $"Соревнуйтесь друг с другом в гонке и получайте скам-койны за попадание в очковую зону (топ-10)"
                                   + $"\nРаспределение скам-койнов:\n"
                                   + string.Join(", ", RaceHelper.PositionToPoints.Values.Take(10).Select(x => x * RaceHelper.PointsToScamCoinsMultiplier))
                                   + "\nГонка начнется через 10 минут"
                               )
                               .AddField(
                                   $"Гонка будет происходить на трассе {raceEvent.Track.Name}",
                                   $"Идеальное время трассы: {TimeSpan.FromMilliseconds(raceEvent.Track.IdealTime).MinSecMs()}"
                               )
                               .WithFooter($"EventId: {raceEvent.Id}")
                               .Build();
        return new DiscordMessageBuilder()
               .WithAllowedMentions(Mentions.All)
               .WithContent(ping)
               .WithEmbed(raceWelcomeEmbed)
               .AddComponents(
                   new DiscordButtonComponent(
                       !raceEvent.Finished
                           ? ButtonStyle.Primary
                           : ButtonStyle.Secondary,
                       InteractionsIds.EventsButtons.Race.BuildId(raceEvent.Id, InteractionsIds.EventsButtons.Race.Join),
                       raceEvent.Participants
                                .Count(x => x.UserId is not null)
                                .ToPluralizedString("участник", "участника", "участников"),
                       raceEvent.Finished,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("monkaSTEER"))
                   )
               );
    }

    private async Task<string> BuildStartingGridMessageAsync(RaceEventDto raceEvent)
    {
        var members = raceEvent
                      .Participants
                      .Where(x => x.UserId is not null)
                      .ToDictionary(x => x.UserId!.Value, x => usersCache.GetMemberByApiIdAsync(x.UserId!.Value));
        var startingGridStringBuilder = new StringBuilder("```");
        for (var i = 0; i < raceEvent.Participants.Length; i++)
        {
            var participant = raceEvent.Participants[i];
            var position = i + 1;
            var driverNameTrigram = participant.Driver.DriverName[..3].ToUpper();
            var memberName = participant.UserId is null
                ? ""
                : members.TryGetValue(participant.UserId.Value, out var memberTask)
                    ? (await memberTask).ServerOrUserName()
                    : "";
            var driverResult = $"{position.AddSpaces(2)} | {driverNameTrigram} | {memberName}";
            startingGridStringBuilder.AppendLine(driverResult);
        }

        startingGridStringBuilder.Append("```");
        return startingGridStringBuilder.ToString();
    }

    private async Task<string> BuildRaceSnapshotMessageAsync(
        Dictionary<Guid, Task<DiscordMember?>> members,
        RaceEventDto raceEvent,
        int currentSectorIndex
    )
    {
        var checkeredFlag = await emotesCache.GetEmoteAsTextAsync("checkered_flag");
        var totalSectorsInLap = raceEvent.Track.AccelerationDifficulty + raceEvent.Track.BreakingDifficulty + raceEvent.Track.CorneringDifficulty;
        var currentSector = raceEvent.Sectors[currentSectorIndex];
        var stringBuilder = new StringBuilder($"```\nКруг {currentSector.CurrentLap} / {raceEvent.TotalLaps}\t");
        var currentSectorIndexOnLap = currentSectorIndex % totalSectorsInLap;
        currentSectorIndexOnLap = currentSectorIndexOnLap == 0 ? totalSectorsInLap : currentSectorIndexOnLap;
        stringBuilder.AppendLine(
            currentSectorIndex == 0
                ? "СТАРТ"
                : currentSector.CurrentLap == raceEvent.TotalLaps && currentSectorIndexOnLap == totalSectorsInLap
                    ? $"{checkeredFlag} {checkeredFlag} {checkeredFlag}"
                    : $"Сектор {currentSectorIndexOnLap} / {totalSectorsInLap}"
        );
        var orderedDrivers = currentSector.DriversOnSector.OrderBy(x => x.TotalTime).ToArray();
        var leader = orderedDrivers.First();
        for (var i = 0; i < orderedDrivers.Length; i++)
        {
            var driver = orderedDrivers[i];
            var position = i + 1;
            var driverNameTrigram = driver.DriverName[..3].ToUpper();
            var participant = raceEvent.Participants.First(x => x.Driver.DriverName == driver.DriverName);
            var memberName = participant.UserId is null
                ? ""
                : members.TryGetValue(participant.UserId!.Value, out var memberTask)
                    ? (await memberTask).ServerOrUserName()
                    : "";
            var totalTime = TimeSpan.FromMilliseconds(driver.TotalTime);
            var currentLapTime = TimeSpan.FromMilliseconds(driver.CurrentLapTime);
            var leaderTrailing = TimeSpan.FromMilliseconds(driver.TotalTime - leader.TotalTime);
            stringBuilder.Append($"{position.AddSpaces(2)} | ")
                         .Append($"{driverNameTrigram} | ")
                         .Append($"{currentLapTime.MinSecMs()} | ")
                         .Append($"{totalTime.MinSecMs()} | ")
                         .Append($"{(i == 0 ? "   LEADER" : $"+{leaderTrailing.MinSecMs()}")} | ")
                         .AppendLine(memberName);
        }

        stringBuilder.Append(
            $"Лучшее время круга: {
                (currentSector.FastestLap.HasValue
                    ? TimeSpan.FromMilliseconds(currentSector.FastestLap.Value).MinSecMs()
                    : "---")
            }"
        ).Append("```");

        return stringBuilder.ToString();
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesCache emotesCache;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IUsersCache usersCache;
}