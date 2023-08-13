using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Transfusion;

public interface ITransfusionEmbedBuilder
{
    Task<DiscordEmbed> BuildAsync(TransfusionEventDto transfusionEvent);
}