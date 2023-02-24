using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus.SlashCommands;
using TelemetryApp.Api.Client.Log;
using VideoLibrary;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

public class PlayYoutubeCommandModule : ApplicationCommandModule
{
    public PlayYoutubeCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        ILoggerClient logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.logger = logger;
    }

    [SlashCommand("youtube", "Воспроизвести звук из видео с ютуба")]
    public async Task PlayAudioFromYoutube(
        InteractionContext context,
        [Option("url", "Ссылка на видео с ютуба")] string url
    )
    {
        const string dir = "./youtube";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var musicMessage = await discordClientWrapper.Messages.RespondAsync(context, "Скачиваю...");

        var youTube = YouTube.Default;
        YouTubeVideo video;
        try
        {
            video = await youTube.GetVideoAsync(url);
        }
        catch
        {
            await discordClientWrapper.Messages.ModifyAsync(musicMessage, "Возникла ошибка при скачивании видео");
            return;
        }

        var fileName = Path.Combine(dir, video.FullName);
        if (!File.Exists(fileName))
        {
            await logger.InfoAsync("Скачиваю {fileName}", fileName);
            await File.WriteAllBytesAsync(fileName, await video.GetBytesAsync());
        }

        await discordClientWrapper.Messages.ModifyAsync(musicMessage, "Добавлено в очередь");
        await Play(context, context.User.Id, fileName);
    }

    private async Task Play(InteractionContext context, ulong userId, string filename)
    {
        var member = await discordClientWrapper.Members.GetAsync(userId);
        if (member.VoiceState?.Channel is null)
        {
            await discordClientWrapper.Messages.RespondAsync(context, "Чел, в (к)анал зайди");
            return;
        }

        await discordClientWrapper.Voice.TryConnectAsync(member.VoiceState.Channel);
        await discordClientWrapper.Voice.PlaySoundAsync(filename);
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly ILoggerClient logger;
}