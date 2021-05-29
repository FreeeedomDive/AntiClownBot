using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var fileName = e.Message.Content.Split(' ').Skip(1).First();
            if (!File.Exists(fileName))
            {
                return "Такого файла нет, чел";
            }

            if (!Voice.TryConnect(e.Channel, out var vnc))
            {
                Voice.Disconnect();
                Voice.TryConnect(e.Channel, out vnc);
            }
            Voice.PlaySound(fileName);
            return "Дело сделано";
        }
    }
}
