using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Google.Cloud.TextToSpeech.V1;

namespace AntiClown.DiscordBot.SlashCommands.Voice;

public class VoiceCommandModule(
    ICommandExecutor commandExecutor,
    IAntiClownDataApiClient antiClownDataApiClient,
    VoiceNextExtension voiceNextExtension,
    ILogger<VoiceCommandModule> logger
)
    : SlashCommandModuleWithMiddlewares(commandExecutor)
{
    [SlashCommand(InteractionsIds.CommandsNames.Voice_Test, "Text-to-speech test")]
    public async Task Voice_Test(
        InteractionContext ctx,
        [Option("text", "The text to test")] string text
    )
    {
        await ExecuteAsync(
            ctx, async () =>
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
                    var client = await TextToSpeechClient.CreateAsync();

                    var input = new SynthesisInput
                    {
                        Text = text,
                    };

                    var voiceSelection = new VoiceSelectionParams
                    {
                        LanguageCode = "ru-RU",
                        SsmlGender = SsmlVoiceGender.Neutral,
                    };

                    var audioConfig = new AudioConfig
                    {
                        AudioEncoding = AudioEncoding.Linear16,
                        SampleRateHertz = 48000,
                    };

                    var response = await client.SynthesizeSpeechAsync(input, voiceSelection, audioConfig);
                    var transmit = connection!.GetTransmitSink();
                    var stream = new MemoryStream(response.AudioContent.ToByteArray());
                    await stream.CopyToAsync(transmit);
                    await connection.WaitForPlaybackFinishAsync();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Real shit happened");
                }
                finally
                {
                    connection?.Disconnect();
                }
            }
        );
    }
}