using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.F1Predictions;
using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.F1Predictions;

public class F1PredictionStartedConsumer : IConsumer<F1PredictionStartedMessageDto>
{
    public F1PredictionStartedConsumer(
        IAntiClownDataApiClient antiClownDataApiClient,
        IDiscordClientWrapper discordClientWrapper,
        IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder,
        ILoggerClient loggerClient
    )
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.f1PredictionsEmbedBuilder = f1PredictionsEmbedBuilder;
        this.loggerClient = loggerClient;
    }

    public async Task Consume(ConsumeContext<F1PredictionStartedMessageDto> context)
    {
        try
        {
            var message = context.Message;
            var embed = f1PredictionsEmbedBuilder.BuildPredictionStarted(message.Name);
            var f1PredictionsChatId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "F1PredictionsChatId");
            await discordClientWrapper.Messages.SendAsync(f1PredictionsChatId, embed);
        }
        catch (Exception e)
        {
            await loggerClient.ErrorAsync(e, "Error in {ConsumerName}", nameof(F1PredictionStartedConsumer));
        }
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder;
    private readonly ILoggerClient loggerClient;
}