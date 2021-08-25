using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Gambling.Commands
{
    public class GambleClose : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public GambleClose(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "close";

        public string Execute(MessageCreateEventArgs e)
        {
            if(Config.CurrentGamble == null)
            {
                return "А ставки-то и нету";
            }
            Config.CurrentGamble.CloseGamble();
            Config.Save();
            return "Сборы закрыты";
        }
    }
}
