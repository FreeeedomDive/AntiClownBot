using System.Diagnostics;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Guilds;
using Loggers;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Voice;

public class VoiceClient : IVoiceClient
{
    public VoiceClient(
        DiscordClient discordClient,
        IGuildsClient guildsClient,
        IGuildSettingsService guildSettingsService,
        ILogger logger
    )
    {
        this.discordClient = discordClient;
        this.guildsClient = guildsClient;
        this.guildSettingsService = guildSettingsService;
        this.logger = logger;
        voiceExtension = discordClient.GetVoiceNext();
    }

    public async Task<DiscordVoiceState[]> GetVoiceStatesInChannelAsync(ulong channelId)
    {
        var guildId = guildSettingsService.GetGuildSettings().GuildId;
        var guild = await discordClient.GetGuildAsync(guildId);

        var voiceStates = guild.VoiceStates.Values.Where(voiceState =>
                voiceState.User is not null
                && !voiceState.User.IsBot
                && voiceState.Channel is not null
                && voiceState.Channel.Id == channelId)
            .ToArray();

        return voiceStates;
    }

    public VoiceNextExtension GetVoice()
    {
        return voiceExtension;
    }

    public async Task<bool> TryConnectAsync(DiscordChannel channel)
    {
        var guild = await guildsClient.GetGuildAsync();
        var con = voiceExtension.GetConnection(guild);
        if (con != null)
        {
            return false;
        }

        await voiceExtension.ConnectAsync(channel);
        return true;
    }

    public async Task PlaySoundAsync(string soundName)
    {
        var guild = await guildsClient.GetGuildAsync();
        connection = voiceExtension.GetConnection(guild);
        if (connection == null)
        {
            logger.Info($"Бот не подключен");
            return;
        }

        if (!File.Exists(soundName))
        {
            logger.Error("Нет файла {Name}", soundName);
            return;
        }

        lock (locker)
        {
            if (connection.IsPlaying)
            {
                logger.Info("Добавлен в очередь {Name}", soundName);
                SoundQueue.Enqueue(soundName);
                return;
            }
        }

        Exception? exc = null;
        logger.Info("Начинаем проигрывать {Name}", soundName);

        try
        {
            await connection.SendSpeakingAsync();

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $@"-i ""{soundName}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var ffmpeg = Process.Start(psi);
            if (ffmpeg == null)
            {
                logger.Error("ffmpeg не стартанул");
                return;
            }
            var ffOut = ffmpeg.StandardOutput.BaseStream;

            var txStream = connection.GetTransmitSink();
            await ffOut.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await connection.WaitForPlaybackFinishAsync();
        }
        catch (Exception ex)
        {
            exc = ex;
        }
        finally
        {
            await connection.SendSpeakingAsync(false);
            logger.Info("Закончили проигрывать {Name}", soundName);
            if (exc != null)
            {
                logger.Error(exc, "Все наебнулось");
            }
            lock (locker)
            {
                if (SoundQueue.Count > 0)
                {
                    logger.Info("Закончили проигрывать {Name}", soundName);
                    PlaySoundAsync(SoundQueue.Dequeue());
                }
                else
                {
                    logger.Info("Ливну через минуту");
                    DisconnectAsync();
                }
            }
        }
    }

    public bool IsPlaying()
    {
        return connection?.IsPlaying ?? false;
    }

    public async Task WaitForPlaybackFinishAsync()
    {
        if (connection == null)
        {
            return;
        }

        await connection.WaitForPlaybackFinishAsync();
    }

    public void ClearQueue()
    {
        lock (locker)
        {
            SoundQueue.Clear();
        }
    }

    public async Task DisconnectAsync()
    {
        var guild = await guildsClient.GetGuildAsync();
        var vnc = voiceExtension.GetConnection(guild);
        if (vnc == null)
        {
            logger.Error("Бот не подключен");
            return;
        }

        await Task.Delay(60 * 1000);
        lock (locker)
        {
            if (SoundQueue.Count > 0 && !vnc.IsPlaying)
            {
                PlaySoundAsync(SoundQueue.Dequeue());
                return;
            }
        }

        if (vnc.IsPlaying)
        {
            logger.Error("Бля, чёт играет, ливать не буду");
            return;
        }

        logger.Info("Ливаю");
        vnc.Disconnect();
    }

    public readonly Queue<string> SoundQueue = new();

    private VoiceNextConnection? connection;
    private readonly DiscordClient discordClient;
    private readonly IGuildsClient guildsClient;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly ILogger logger;
    private readonly VoiceNextExtension voiceExtension;
    private readonly object locker = new();
}