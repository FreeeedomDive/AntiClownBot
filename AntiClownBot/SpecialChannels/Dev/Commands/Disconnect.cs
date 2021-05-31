using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class Disconnect : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public Disconnect(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }

        public string Name => "disconnect";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            Voice.SoundQueue.Clear();
            var vnc = Voice.VoiceExtension.GetConnection(Utility.Client.Guilds[277096298761551872]);
            vnc.Disconnect();
            return "Ну я вышел, и чё";
        }
    }
}
