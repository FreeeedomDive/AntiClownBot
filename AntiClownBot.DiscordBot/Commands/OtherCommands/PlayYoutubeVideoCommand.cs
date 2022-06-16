using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using Loggers;
using DSharpPlus.EventArgs;
using VideoLibrary;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands
{
    public class PlayYoutubeVideoCommand : ICommand
    {
        public PlayYoutubeVideoCommand(
            IDiscordClientWrapper discordClientWrapper,
            ILogger logger
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.logger = logger;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            const string dir = "./youtube";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var musicMessage = await discordClientWrapper.Messages.RespondAsync(e.Message, "Скачиваю...");
            var link = string.Join(" ", e.Message.Content.Split(' ').Skip(1));

            var youTube = YouTube.Default;
            YouTubeVideo video;
            try
            {
                video = await youTube.GetVideoAsync(link);
            }
            catch
            {
                await discordClientWrapper.Messages.ModifyAsync(musicMessage, "Возникла ошибка при скачивании видео");
                return;
            }

            var fileName = Path.Combine(dir, video.FullName);
            if (!File.Exists(fileName))
            {
                logger.Info($"Скачиваю {fileName}");
                await File.WriteAllBytesAsync(fileName, await video.GetBytesAsync());
            }

            await discordClientWrapper.Messages.ModifyAsync(musicMessage,"Добавлено в очередь");
            await Play(e, e.Author.Id, fileName);
        }

        private async Task Play(MessageCreateEventArgs e, ulong userId, string filename)
        {
            var member = await discordClientWrapper.Members.GetAsync(userId);
            if (member.VoiceState == null || member.VoiceState.Channel == null)
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, "Чел, в (к)анал зайди");
                return;
            }

            await discordClientWrapper.Voice.TryConnectAsync(member.VoiceState.Channel);
            await discordClientWrapper.Voice.PlaySoundAsync(filename);
        }

        public Task<string> Help() => Task.FromResult("Запуск музыки с ютуба");
        public string Name => "play";

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly ILogger logger;
    }
}