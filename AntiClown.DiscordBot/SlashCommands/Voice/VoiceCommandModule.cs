using System.Text;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;

namespace AntiClown.DiscordBot.SlashCommands.Voice;

public class VoiceCommandModule(
    ICommandExecutor commandExecutor,
    IAntiClownDataApiClient antiClownDataApiClient,
    VoiceNextExtension voiceNextExtension)
    : SlashCommandModuleWithMiddlewares(commandExecutor)
{
    [SlashCommand(InteractionsIds.CommandsNames.Voice_Test, "Text-to-speech test")]
    public async Task Voice_Test(InteractionContext ctx,
        [Option("text", "The text to test")] string text)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var guildId =
                await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
            var guild = await ctx.Client.GetGuildAsync(guildId);
            var connection = voiceNextExtension.GetConnection(guild);
            var channel = ctx.Member.VoiceState?.Channel;
            if (channel == null)
            {
                await RespondToInteractionAsync(ctx, "Сначала зайди в войс-канал");
                return;
            }

            if (connection == null)
            {
                await voiceNextExtension.ConnectAsync(channel);
            }

            try
            {
                using var voiceApiClient = new HttpClient();
                var response = await voiceApiClient.PostAsync("http://localhost:5002/api/tts",
                    new StringContent($"{{\"text\":\"{text}\"}}", Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                var audioData = await response.Content.ReadAsStreamAsync();
                var transmit = connection!.GetTransmitSink();
                await audioData.CopyToAsync(transmit);
                await connection.WaitForPlaybackFinishAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                connection?.Disconnect();
            }
        });
    }
}