using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class PlayMusic : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public PlayMusic(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "play";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var fileName = string.Join(" ", e.Message.Content.Split(' ').Skip(1));
            if (!File.Exists(fileName))
            {
                return "Такого файла нет, чел";
            }

            var member = DiscordClient.Guilds[Constants.GuildId].GetMemberAsync(user.DiscordId).Result;
            if (member.VoiceState == null || member.VoiceState.Channel == null) return "Чел, в (к)анал зайди";
            new Thread(() =>
            {
                Voice.TryConnect(member.VoiceState.Channel, out var vnc);
                Voice.PlaySound(fileName);
            })
            {
                IsBackground = true
            }.Start();
            return "Дело сделано";
        }
    }
}
