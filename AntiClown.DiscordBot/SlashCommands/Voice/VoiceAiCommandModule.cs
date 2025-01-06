using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Ai.Client;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.DiscordBot.Utility.Locks;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Google.Cloud.TextToSpeech.V1;

namespace AntiClown.DiscordBot.SlashCommands.Voice;

public class VoiceAiCommandModule(
    ICommandExecutor commandExecutor,
    IAntiClownDataApiClient antiClownDataApiClient,
    VoiceNextExtension voiceNextExtension,
    IGeminiAiClient geminiAiClient,
    ILocker locker,
    ILogger<VoiceAiCommandModule> logger
)
    : SlashCommandModuleWithMiddlewares(commandExecutor)
{
    [SlashCommand(InteractionsIds.CommandsNames.VoiceAi, "Разговоры с батюшкой")]
    public async Task Voice_Test(
        InteractionContext ctx,
        [Option("text", "Спроси у ИИ...")] string text
    )
    {
        await ExecuteAsync(
            ctx, () =>
            {
                return locker.DoInLockAsync(
                    nameof(VoiceAiCommandModule), async () =>
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
                            var aiResponse = await geminiAiClient.GetResponseAsync(text);
                            var client = await TextToSpeechClient.CreateAsync();

                            var input = new SynthesisInput
                            {
                                Text = aiResponse,
                            };

                            var voice = new VoiceSelectionParams
                            {
                                LanguageCode = "ru-RU",
                                SsmlGender = SsmlVoiceGender.Neutral,
                                Name = "ru-RU-Standard-D",
                            };

                            var audioConfig = new AudioConfig
                            {
                                AudioEncoding = AudioEncoding.Linear16,
                                SampleRateHertz = 48000,
                            };

                            var response = await client.SynthesizeSpeechAsync(input, voice, audioConfig);
                            connection ??= await channel.ConnectAsync();
                            var transmit = connection!.GetTransmitSink();
                            var contentBytes = response.AudioContent.ToByteArray();
                            var stream = ToStereoStream(contentBytes);
                            await stream.CopyToAsync(transmit);
                            await stream.FlushAsync();
                            await connection.WaitForPlaybackFinishAsync();
                            await RespondToInteractionAsync(ctx, "Вот и поговорили");
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, $"{nameof(VoiceAiCommandModule)} error");
                            await RespondToInteractionAsync(ctx, "Я споткнулся");
                        }
                        finally
                        {
                            connection?.Disconnect();
                        }
                    }
                );
            }
        );
    }

    private static MemoryStream ToStereoStream(byte[] monoStreamBytes)
    {
        var stereoStream = new MemoryStream();
        for (var i = 0; i < monoStreamBytes.Length; i += 2)
        {
            var buffer = new byte[4];
            var outIndex = 0;
            buffer[outIndex++] = monoStreamBytes[i];
            buffer[outIndex++] = monoStreamBytes[i + 1];

            // Copy the same 2 bytes again for the right channel
            buffer[outIndex++] = monoStreamBytes[i];
            buffer[outIndex] = monoStreamBytes[i + 1];
            stereoStream.Write(buffer, 0, buffer.Length);
        }

        stereoStream.Position = 0;
        return stereoStream;
    }
}