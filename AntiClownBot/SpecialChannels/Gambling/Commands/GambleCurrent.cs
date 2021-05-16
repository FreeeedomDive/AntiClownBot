using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Gambling.Commands
{
    public class GambleCurrent : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public GambleCurrent(DiscordClient client, Configuration configuration)
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
