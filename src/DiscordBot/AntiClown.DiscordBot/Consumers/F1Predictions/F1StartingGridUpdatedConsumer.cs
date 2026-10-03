using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.F1Predictions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.F1Predictions;

public class F1StartingGridUpdatedConsumer(
    IAntiClownDataApiClient antiClownDataApiClient,
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IDiscordClientWrapper discordClientWrapper,
    IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder,
    ILogger<F1StartingGridUpdatedConsumer> logger
) : IConsumer<F1StartingGridUpdatedMessageDto>
{
    public async Task Consume(ConsumeContext<F1StartingGridUpdatedMessageDto> context)
    {
        try
        {
            var race = await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(context.Message.RaceId);
            var embed = f1PredictionsEmbedBuilder.BuildStartingGridUpdated(race);
            var chatId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "F1PredictionsChatId");
            await discordClientWrapper.Messages.SendAsync(chatId, embed);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error in {ConsumerName}", nameof(F1StartingGridUpdatedConsumer));
        }
    }
}
