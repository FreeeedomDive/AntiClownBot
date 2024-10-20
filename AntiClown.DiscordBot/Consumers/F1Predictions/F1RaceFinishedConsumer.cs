﻿using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.F1Predictions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.F1Predictions;

public class F1RaceFinishedConsumer : IConsumer<F1RaceFinishedMessageDto>
{
    public F1RaceFinishedConsumer(
        IAntiClownDataApiClient antiClownDataApiClient,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IDiscordClientWrapper discordClientWrapper,
        IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder,
        ILoggerClient loggerClient
    )
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.f1PredictionsEmbedBuilder = f1PredictionsEmbedBuilder;
        this.loggerClient = loggerClient;
    }

    public async Task Consume(ConsumeContext<F1RaceFinishedMessageDto> context)
    {
        try
        {
            var race = await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(context.Message.RaceId);
            var results = await antiClownEntertainmentApiClient.F1Predictions.ReadResultsAsync(context.Message.RaceId);
            var embed = f1PredictionsEmbedBuilder.BuildRaceFinished(race, results);
            var f1PredictionsChatId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "F1PredictionsChatId");
            await discordClientWrapper.Messages.SendAsync(f1PredictionsChatId, embed);
        }
        catch(Exception e)
        {
            await loggerClient.ErrorAsync(e, "Error in {ConsumerName}", nameof(F1RaceFinishedConsumer));
        }
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder;
    private readonly ILoggerClient loggerClient;
}