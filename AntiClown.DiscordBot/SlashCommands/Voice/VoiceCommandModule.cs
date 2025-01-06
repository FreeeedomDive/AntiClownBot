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
                        SampleRateHertz = 16000,
                    };

                    var response = await client.SynthesizeSpeechAsync(input, voice, audioConfig);
                    var ttsData = response.AudioContent.ToByteArray();
                    logger.LogInformation("Я получил ответ от гугла {length} байт", ttsData.Length);

                    using var ffmpeg = new Process();
                    ffmpeg.StartInfo.FileName = "ffmpeg";
                    ffmpeg.StartInfo.Arguments = "-i pipe:0 -f s16le -ar 48000 -vn -sn -dn -ac 2 pipe:1 -loglevel quiet";
                    ffmpeg.StartInfo.UseShellExecute = false;
                    ffmpeg.StartInfo.RedirectStandardInput = true;
                    ffmpeg.StartInfo.RedirectStandardOutput = true;
                    ffmpeg.StartInfo.CreateNoWindow = true;
                    ffmpeg.Start();

                    var ffmpegIn = ffmpeg.StandardInput.BaseStream;
                    var ffmpegOut = ffmpeg.StandardOutput.BaseStream;

                    connection ??= await channel.ConnectAsync();
                    using var discordStream = connection.GetTransmitSink();

                    const int bufferSize = 8192;
                    var totalBytesRead = 0;
                    var readBuffer = new byte[8192];
                    foreach (var batch in ttsData.Batch(bufferSize))
                    {
                        var batchData = batch.ToArray();
                        await ffmpegIn.WriteAsync(batchData);
                        int bytesRead;
                        while ((bytesRead = await ffmpegOut.ReadAsync(readBuffer)) > 0)
                        {
                            await discordStream.WriteAsync(readBuffer, 0, bytesRead);
                            logger.LogInformation("Я высрал {TotalBytes} байтов", totalBytesRead);
                            totalBytesRead += bytesRead;
                        }
                    }
                    await discordStream.FlushAsync();

                    ffmpegIn.Close();
                    ffmpegOut.Close();

                    await ffmpeg.WaitForExitAsync();
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