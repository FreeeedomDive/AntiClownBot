using System.Diagnostics;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Tools.Utility.Extensions;
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

                try
                {
                    var client = await TextToSpeechClient.CreateAsync();
                    logger.LogInformation("Я создал клиента");

                    var input = new SynthesisInput
                    {
                        Text = text,
                    };

                    var voice = new VoiceSelectionParams
                    {
                        LanguageCode = "ru-RU",
                        SsmlGender = SsmlVoiceGender.Neutral,
                    };

                    var audioConfig = new AudioConfig
                    {
                        AudioEncoding = AudioEncoding.Linear16,
                        SampleRateHertz = 48000,
                    };

                    var response = await client.SynthesizeSpeechAsync(input, voice, audioConfig);
                    var transmit = connection!.GetTransmitSink();
                    var contentBytes = response.AudioContent.ToByteArray();
                    var stream = ToStereoStream(contentBytes);
                    await stream.CopyToAsync(transmit);
                    await stream.FlushAsync();
                    await connection.WaitForPlaybackFinishAsync();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "VoiceCommandModule error");
                }
                finally
                {
                    connection?.Disconnect();
                    await RespondToInteractionAsync(ctx, text);
                }
            }
        );
    }

    private static Stream ToStereoStream(byte[] monoStreamBytes)
    {
        var stereoStream = new MemoryStream();
        for (var i = 0; i < monoStreamBytes.Length; i += 2)
        {
            var buffer = new byte[4];
            var outIndex = 0;
            buffer[outIndex++] = buffer[i];
            buffer[outIndex++] = buffer[i + 1];

            // Copy the same 2 bytes again for the right channel
            buffer[outIndex++] = buffer[i];
            buffer[outIndex] = buffer[i + 1];
            stereoStream.Write(buffer, 0, buffer.Length);
        }

        stereoStream.Position = 0;
        return stereoStream;
    }
}