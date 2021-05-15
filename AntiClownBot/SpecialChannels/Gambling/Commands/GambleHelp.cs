using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.Gambling.Commands
{
    public class GambleHelp : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public GambleHelp(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "current";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            return Config.CurrentGamble != null ? Config.CurrentGamble.ToString() : "В данный момент нет активной ставки";
        }
    }
}
