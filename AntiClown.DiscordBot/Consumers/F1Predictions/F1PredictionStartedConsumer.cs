using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.F1Predictions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.F1Predictions;

public class F1PredictionStartedConsumer(
    IAntiClownDataApiClient antiClownDataApiClient,
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IDiscordClientWrapper discordClientWrapper,
    IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder,
    ILogger<F1PredictionStartedConsumer> logger
)
    : IConsumer<F1PredictionStartedMessageDto>
{
    public async Task Consume(ConsumeContext<F1PredictionStartedMessageDto> context)
    {
        try
        {
            var message = context.Message;
            var race = await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(message.RaceId);
            var embed = f1PredictionsEmbedBuilder.BuildPredictionStarted(race.FullName());
            var f1PredictionsChatId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "F1PredictionsChatId");
            await discordClientWrapper.Messages.SendAsync(f1PredictionsChatId, embed);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in {ConsumerName}", nameof(F1PredictionStartedConsumer));
        }
    }
}