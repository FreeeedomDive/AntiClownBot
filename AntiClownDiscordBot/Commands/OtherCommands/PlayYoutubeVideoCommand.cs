using System.IO;
using System.Linq;
using System.Threading;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using VideoLibrary;

namespace AntiClownBot.Commands.OtherCommands
{
    public class PlayYoutubeVideoCommand : BaseCommand
    {
        public PlayYoutubeVideoCommand(DiscordClient client, Configuration configuration): base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            DiscordMessage musicMessage = null;
            const string dir = "./youtube";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            new Thread(async () =>
            {
                var link = string.Join(" ", e.Message.Content.Split(' ').Skip(1));

                var youTube = YouTube.Default;
                YouTubeVideo video;
                try
                {
                    video = await youTube.GetVideoAsync(link);
                }
                catch
                {
                    await musicMessage.ModifyAsync("Возникла ошибка при скачивании видео");
                    return;
                }
                var fileName = Path.Combine(dir, video.FullName);
                if (!File.Exists(fileName))
                {
                    NLogWrapper.GetDefaultLogger().Info($"Скачиваю {fileName}");
                    await File.WriteAllBytesAsync(fileName, await video.GetBytesAsync());
                }
                Play(e, e.Author.Id, fileName);
                await musicMessage.ModifyAsync("Добавлено в очередь");
            })
            {
                IsBackground = true
            }.Start();
            musicMessage = await e.Message.RespondAsync("Скачиваю...");
        }

        public override string Help() => "Запуск музыки с ютуба";

        private async void Play(MessageCreateEventArgs e, ulong userId, string filename)
        {
            var member = await DiscordClient.Guilds[Constants.GuildId].GetMemberAsync(userId);
            if (member.VoiceState == null || member.VoiceState.Channel == null)
            {
                await e.Message.RespondAsync("Чел, в (к)анал зайди");
                return;
            }
            Voice.TryConnect(member.VoiceState.Channel, out var vnc);
            Voice.PlaySound(filename);
        }
    }
}