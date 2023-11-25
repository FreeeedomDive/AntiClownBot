using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.F1PredictionsStats;

public interface IF1PredictionStatsEmbedBuilder
{
    DiscordEmbed Build(MostPickedDriversByUsersStatsDto mostPickedDriversByUsersStats);
}