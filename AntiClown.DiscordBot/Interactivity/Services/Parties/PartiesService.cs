using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Parties;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Domain.Parties;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Utility.Locks;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Parties;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.Interactivity.Services.Parties;

public class PartiesService : IPartiesService
{
    public PartiesService(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IDiscordClientWrapper discordClientWrapper,
        IInteractivityRepository interactivityRepository,
        IPartyEmbedBuilder partyEmbedBuilder,
        IEmotesCache emotesCache,
        IUsersCache usersCache,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILocker locker
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.interactivityRepository = interactivityRepository;
        this.partyEmbedBuilder = partyEmbedBuilder;
        this.emotesCache = emotesCache;
        this.usersCache = usersCache;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.locker = locker;
    }

    public async Task AddPlayerAsync(Guid partyId, ulong memberId)
    {
        var userId = await usersCache.GetApiIdByMemberIdAsync(memberId);
        await antiClownEntertainmentApiClient.Parties.JoinAsync(partyId, userId);
    }

    public async Task RemovePlayerAsync(Guid partyId, ulong memberId)
    {
        var userId = await usersCache.GetApiIdByMemberIdAsync(memberId);
        await antiClownEntertainmentApiClient.Parties.LeaveAsync(partyId, userId);
    }

    public async Task ClosePartyAsync(Guid partyId, ulong memberId)
    {
        var party = await antiClownEntertainmentApiClient.Parties.ReadAsync(partyId);
        var userId = await usersCache.GetApiIdByMemberIdAsync(memberId);
        if (party.CreatorId != userId)
        {
            return;
        }

        await antiClownEntertainmentApiClient.Parties.CloseAsync(partyId);
    }

    public async Task CreateOrUpdateAsync(Guid partyId)
    {
        await locker.DoInLockAsync(
            $"Party {partyId}", async () =>
            {
                var party = await antiClownEntertainmentApiClient.Parties.ReadAsync(partyId);
                var interactivity = await interactivityRepository.TryReadAsync<PartyDetails>(partyId);
                var messageBuilder = await BuildMessageAsync(party);
                var partyChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "PartyChannelId");
                DiscordMessage message;
                if (interactivity is null)
                {
                    message = await discordClientWrapper.Messages.SendAsync(partyChannelId, messageBuilder);
                    var thread = await discordClientWrapper.Messages.CreateThreadAsync(message, $"{party.Name} {party.Description}");
                    var author = await usersCache.GetMemberByApiIdAsync(party.CreatorId);
                    interactivity = new Interactivity<PartyDetails>
                    {
                        Id = partyId,
                        CreatedAt = party.CreatedAt,
                        Type = InteractivityType.Party,
                        AuthorId = author!.Id,
                        MessageId = message.Id,
                        Details = new PartyDetails
                        {
                            LastPing = null,
                            ThreadId = thread.Id,
                        },
                    };
                    await interactivityRepository.CreateAsync(interactivity);
                }
                else
                {
                    message = await discordClientWrapper.Messages.FindMessageAsync(partyChannelId, interactivity.MessageId);
                    await discordClientWrapper.Messages.ModifyAsync(message, messageBuilder);
                }

                // даем пинговать о полном пати только когда пати полностью собирается первый раз
                if (party.IsOpened && party.Participants.Count == party.MaxPlayers && interactivity.Details!.LastPing is null)
                {
                    var readyPlayersMentions = await BuildReadyPlayersMentionsStringAsync(party);
                    var fullPartyContent =
                        $"{(party.FirstFullPartyAt ?? DateTime.UtcNow).GetDifferenceTimeSpan(party.CreatedAt).ToTimeDiffString()}\n{readyPlayersMentions}";
                    await SendMessageToThreadAsync(interactivity.Details.ThreadId, $"Набрано полное пати за {fullPartyContent}");
                    interactivity.Details.LastPing = DateTime.UtcNow;
                    await interactivityRepository.UpdateAsync(interactivity);
                }
            }
        );
    }

    public async Task PingReadyPlayersAsync(Guid partyId, ulong memberId)
    {
        var userId = await usersCache.GetApiIdByMemberIdAsync(memberId);
        var party = await antiClownEntertainmentApiClient.Parties.ReadAsync(partyId);
        if (!party.Participants.Contains(userId))
        {
            return;
        }
        var interactivity = await interactivityRepository.TryReadAsync<PartyDetails>(partyId);
        var readyPlayersMentions = await BuildReadyPlayersMentionsStringAsync(party);
        await SendMessageToThreadAsync(interactivity!.Details!.ThreadId, readyPlayersMentions);
    }

    private async Task<string> BuildReadyPlayersMentionsStringAsync(PartyDto party)
    {
        var readyPlayersTasks = party.Participants
                                     .Take(party.MaxPlayers)
                                     .Select(playerId => usersCache.GetMemberByApiIdAsync(playerId));
        var readyPlayers = await Task.WhenAll(readyPlayersTasks);
        var readyPlayersMentions = readyPlayers.Select(x => x?.Mention);
        return string.Join("\n", readyPlayersMentions);
    }

    private async Task SendMessageToThreadAsync(ulong threadId, string content)
    {
        var messageBuilder = new DiscordMessageBuilder()
                                 .WithAllowedMention(UserMention.All)
                                 .WithContent(content);
        await discordClientWrapper.Messages.SendAsync(threadId, messageBuilder, true);
    }

    private async Task<DiscordMessageBuilder> BuildMessageAsync(PartyDto party)
    {
        var role = await discordClientWrapper.Roles.FindRoleAsync(party.RoleId);
        var embed = await partyEmbedBuilder.BuildPartyEmbedAsync(party);
        return new DiscordMessageBuilder()
               .WithAllowedMentions(Mentions.All)
               .WithContent(role.Mention)
               .WithEmbed(embed)
               .AddComponents(
                   new DiscordButtonComponent
                   (
                       ButtonStyle.Success,
                       InteractionsIds.PartyButtons.BuildId(party.Id, InteractionsIds.PartyButtons.Join),
                       "Присоединиться",
                       !party.IsOpened,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("YEP"))
                   ),
                   new DiscordButtonComponent
                   (
                       ButtonStyle.Danger,
                       InteractionsIds.PartyButtons.BuildId(party.Id, InteractionsIds.PartyButtons.Leave),
                       "Выйти",
                       !party.IsOpened,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("NOPE"))
                   )
               )
               .AddComponents(
                   new DiscordButtonComponent
                   (
                       ButtonStyle.Secondary,
                       InteractionsIds.PartyButtons.BuildId(party.Id, InteractionsIds.PartyButtons.Ping),
                       "Пинг участников",
                       !party.IsOpened,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("peepoPing"))
                   ),
                   new DiscordButtonComponent
                   (
                       ButtonStyle.Secondary,
                       InteractionsIds.PartyButtons.BuildId(party.Id, InteractionsIds.PartyButtons.Close),
                       "Закрыть сбор",
                       !party.IsOpened,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("BibleThump"))
                   )
               );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesCache emotesCache;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IPartyEmbedBuilder partyEmbedBuilder;
    private readonly IUsersCache usersCache;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly ILocker locker;
}