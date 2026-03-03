using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.GuessNumber;

public interface IGuessNumberEmbedBuilder
{
    Task<DiscordEmbed> BuildAsync(GuessNumberEventDto guessNumberEvent);
}