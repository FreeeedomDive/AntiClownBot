using AntiClown.Api.Dto.Economies;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Tributes;

public interface ITributeEmbedBuilder
{
    Task<DiscordEmbed> BuildForSuccessfulTributeAsync(TributeDto tribute);
    Task<DiscordEmbed> BuildForTributeOnCoolDownAsync();
}