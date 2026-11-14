using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.F1Predictions;

public interface IF1PredictionsEmbedBuilder
{
    Task<DiscordEmbed> BuildBingoCompletedAsync(Guid userId);
}
