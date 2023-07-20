using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain.Parties;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Dto.Parties;
using AntiClown.Tools.Utility.Extensions;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.EmbedBuilders.Parties;

public class PartyEmbedBuilder : IPartyEmbedBuilder
{
    public PartyEmbedBuilder(
        IDiscordClientWrapper discordClientWrapper,
        IUsersCache usersCache,
        IEmotesCache emotesCache,
        IInteractivityRepository interactivityRepository,
        IOptions<DiscordOptions> discordOptions
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
        this.interactivityRepository = interactivityRepository;
        this.discordOptions = discordOptions;
    }

    public async Task<DiscordEmbed> BuildPartyEmbedAsync(PartyDto partyDto)
    {
        var role = await discordClientWrapper.Roles.FindRoleAsync(partyDto.RoleId);
        var embedBuilder = new DiscordEmbedBuilder()
                           .WithTitle($"СБОР ПАТИ {partyDto.Name} {partyDto.Description}")
                           .WithColor(partyDto.IsOpened ? role.Color : DiscordColor.Black);

        var players = await Task.WhenAll(partyDto.Participants.Select(x => usersCache.GetMemberByApiIdAsync(x)));
        var playersList = players
                          .Take(partyDto.MaxPlayers)
                          .Select((player, index) => $"{index + 1}. {player.ServerOrUserName()}")
                          .ToArray();

        embedBuilder.AddField(
            $"{Math.Min(partyDto.MaxPlayers, players.Length)} / {partyDto.MaxPlayers} игроков",
            players.Length == 0 ? "Пока никто не записался..." : string.Join("\n", playersList)
        );

        if (players.Length > partyDto.MaxPlayers)
        {
            var playersQueueList = players
                                   .Skip(partyDto.MaxPlayers)
                                   .Select(
                                       (player, index) =>
                                           $"{partyDto.MaxPlayers + index + 1}. {player.ServerOrUserName()}"
                                   );

            embedBuilder.AddField("Очередь", string.Join("\n", playersQueueList));
        }

        embedBuilder.WithFooter(partyDto.IsOpened ? $"PartyId: {partyDto.Id}" : "СБОР ЗАКРЫТ");

        return embedBuilder.Build();
    }

    public async Task<DiscordEmbed> BuildOpenedPartiesEmbedAsync(PartyDto[] parties)
    {
        var embedBuilder = new DiscordEmbedBuilder();
        var partyMessagesById = parties.Select(x => interactivityRepository.TryReadAsync<PartyDetails>(x.Id).GetAwaiter().GetResult())
                                .Where(x => x is not null)
                                .OrderBy(x => x!.CreatedAt)
                                .ToDictionary(x => x!.Id, x => x!.MessageId);
        var partiesLinks = parties
                           .Where(x => partyMessagesById.TryGetValue(x.Id, out _))
                           .ToDictionary(
                               p => p,
                               p => $"https://discord.com/channels/{discordOptions.Value.GuildId}/{discordOptions.Value.PartyChannelId}/{partyMessagesById[p.Id]}"
                           );

        embedBuilder.WithTitle("ТЕКУЩИЕ ПАТИ");
        if (parties.Length == 0)
        {
            embedBuilder = embedBuilder
                           .WithColor(DiscordColor.DarkRed).AddField(
                               $"{await emotesCache.GetEmoteAsTextAsync("BibleThump")}",
                               "Сейчас никто не играет"
                           );
        }
        else
        {
            embedBuilder = embedBuilder
                .WithColor(DiscordColor.DarkGreen);
            partiesLinks.ForEach(
                kv => embedBuilder.AddField(
                    $"{kv.Key.Name} - {kv.Key.Participants.Count} / {kv.Key.MaxPlayers} игроков",
                    $"[Ссылка]({kv.Value})"
                )
            );
        }

        return embedBuilder.Build();
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IEmotesCache emotesCache;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IUsersCache usersCache;
}