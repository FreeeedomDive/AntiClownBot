using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.RemoveCoolDowns;

public interface IRemoveCoolDownsEmbedBuilder
{
    Task<DiscordEmbed> BuildAsync(RemoveCoolDownsEventDto removeCoolDownsEvent);
}