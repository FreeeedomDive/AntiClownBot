﻿using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.F1Predictions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.F1Predictions;

public class F1PredictionsConsumer : IConsumer<F1UserPredictionUpdatedMessageDto>
{
    public F1PredictionsConsumer(
        IAntiClownDataApiClient antiClownDataApiClient,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IDiscordClientWrapper discordClientWrapper,
        IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder
    )
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.f1PredictionsEmbedBuilder = f1PredictionsEmbedBuilder;
    }

    public async Task Consume(ConsumeContext<F1UserPredictionUpdatedMessageDto> context)
    {
        var message = context.Message;
        var race = await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(message.RaceId);
        var prediction = race.Predictions.FirstOrDefault(x => x.UserId == message.UserId);
        if (prediction is null)
        {
            return;
        }

        var embed = await f1PredictionsEmbedBuilder.BuildPredictionUpdatedAsync(message, race, prediction);
        var f1PredictionsChatId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "F1PredictionsChatId");
        await discordClientWrapper.Messages.SendAsync(f1PredictionsChatId, embed);
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder;
}