using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Messages.Dto.F1Predictions;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.F1Predictions;

public interface IF1PredictionsEmbedBuilder
{
    Task<DiscordEmbed> BuildPredictionUpdatedAsync(F1UserPredictionUpdatedMessageDto message, F1RaceDto race, F1PredictionDto prediction);
    DiscordEmbed BuildResultsUpdated(F1RaceDto race);
    DiscordEmbed BuildRaceFinished(F1PredictionUserResultDto[] results);
}