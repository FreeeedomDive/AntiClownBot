using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Lottery;

public interface ILotteryEmbedBuilder
{
    Task<DiscordEmbed> BuildAsync(LotteryEventDto lottery);
}