using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Google.Cloud.TextToSpeech.V1;
using NAudio.Wave;

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
                    connection ??= await channel.ConnectAsync();

                    var client = await TextToSpeechClient.CreateAsync();
                    logger.LogInformation("Я создал клиента");

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
                        SampleRateHertz = 16000,
                    };
                    var response = await client.SynthesizeSpeechAsync(input, voiceSelection, audioConfig);
                    using var ttsStream = new MemoryStream(response.AudioContent.ToByteArray());
                    await using var waveReader = new WaveFileReader(ttsStream);
                    var desiredFormat = new WaveFormat(48000, 16, 1);
                    using var resampler = new MediaFoundationResampler(waveReader, desiredFormat);
                    resampler.ResamplerQuality = 60;
                    using var transmit = connection.GetTransmitSink();
                    var buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await transmit.WriteAsync(buffer, 0, bytesRead);
                    }

                    await transmit.FlushAsync();
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
}