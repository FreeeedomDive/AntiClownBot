using System.IO;
using System.Linq;
using System.Threading;
using DSharpPlus;
using DSharpPlus.EventArgs;
using VideoLibrary;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class PlayYoutubeVideo : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;

        public PlayYoutubeVideo(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }

        public string Name => "playyt";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
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
                    await e.Message.RespondAsync("Возникла ошибка при скачивании видео");
                    return;
                }
                var fileName = Path.Combine(dir, video.FullName);
                if (!File.Exists(fileName))
                {
                    NLogWrapper.GetDefaultLogger().Info($"Скачиваю {fileName}");
                    await File.WriteAllBytesAsync(fileName, await video.GetBytesAsync());
                }
                Play(e, user, fileName);
            })
            {
                IsBackground = true
            }.Start();
            return "Обрабатываю...";
        }

        private async void Play(MessageCreateEventArgs e, SocialRatingUser user, string filename)
        {
            var member = DiscordClient.Guilds[Constants.GuildId].GetMemberAsync(user.DiscordId).Result;
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