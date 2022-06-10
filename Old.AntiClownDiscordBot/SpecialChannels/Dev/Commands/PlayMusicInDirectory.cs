using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class PlayMusicInDirectory : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        
        public PlayMusicInDirectory(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        
        public string Name => "playdir";

        public string Execute(MessageCreateEventArgs e)
        {
            var directory = string.Join(" ", e.Message.Content.Split(' ').Skip(1));
            if (!Directory.Exists(directory))
            {
                return "Такого файла нет, чел";
            }
            
            var extensions = new List<string> {".mp3", ".flac", ".mp4"};
            var files = Directory
                .GetFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0)
                .OrderBy(_ => Randomizer.GetRandomNumberBetween(0, 1000000));

            var member = Configuration.GetServerMember(e.Author.Id);
            if (member.VoiceState == null || member.VoiceState.Channel == null) return "Чел, в (к)анал зайди";
            new Thread(() =>
            {
                Voice.TryConnect(member.VoiceState.Channel, out var vnc);
                var count = 0;
                foreach (var file in files)
                {
                    Voice.SoundQueue.Enqueue(file);
                    count++;
                }
                NLogWrapper.GetDefaultLogger().Info($"Добавлено {count} треков");
                Voice.PlaySound(Voice.SoundQueue.Dequeue());
            })
            {
                IsBackground = true
            }.Start();
            return "Дело сделано";
        }
    }
}