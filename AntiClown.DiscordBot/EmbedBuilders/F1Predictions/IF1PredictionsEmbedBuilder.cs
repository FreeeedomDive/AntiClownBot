﻿using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Messages.Dto.F1Predictions;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.F1Predictions;

public interface IF1PredictionsEmbedBuilder
{
    DiscordEmbed BuildPredictionStarted(string raceName);
    Task<DiscordEmbed> BuildPredictionUpdatedAsync(F1UserPredictionUpdatedMessageDto message, F1RaceDto race, F1PredictionDto prediction);
    DiscordEmbed BuildResultsUpdated(F1RaceDto race);
    DiscordEmbed BuildRaceFinished(F1RaceDto race, F1PredictionUserResultDto[] results);
    Task<DiscordEmbed> BuildBingoCompletedAsync(Guid userId);
}