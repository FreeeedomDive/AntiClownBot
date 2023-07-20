using AntiClown.Entertainment.Api.Dto.Parties;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Parties;

public interface IPartyEmbedBuilder
{
    Task<DiscordEmbed> BuildPartyEmbedAsync(PartyDto partyDto);
    Task<DiscordEmbed> BuildOpenedPartiesEmbedAsync(PartyDto[] parties);
}