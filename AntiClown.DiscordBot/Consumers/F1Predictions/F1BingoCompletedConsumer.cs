using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.F1Predictions;
using AntiClown.Messages.Dto.F1Predictions;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.F1Predictions;

public class F1BingoCompletedConsumer(
    IAntiClownDataApiClient antiClownDataApiClient,
    IDiscordClientWrapper discordClientWrapper,
    IF1PredictionsEmbedBuilder f1PredictionsEmbedBuilder,
    ILogger<F1BingoCompletedConsumer> logger
) : IConsumer<F1BingoBoardCompletedMessageDto>
{
    public async Task Consume(ConsumeContext<F1BingoBoardCompletedMessageDto> context)
    {
        try
        {
            var message = context.Message;
            var embed = await f1PredictionsEmbedBuilder.BuildBingoCompletedAsync(message.UserId);
            var f1PredictionsChatId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "F1PredictionsChatId");
            await discordClientWrapper.Messages.SendAsync(f1PredictionsChatId, embed);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in {ConsumerName}", nameof(F1BingoCompletedConsumer));
        }
    }
}