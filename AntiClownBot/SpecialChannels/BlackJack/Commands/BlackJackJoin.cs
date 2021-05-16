using AntiClownBot.SpecialChannels;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Linq;

namespace AntiClownBot.SpecialChannels.BlackJack.Commands
{
    public class BlackJackJoin : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public BlackJackJoin(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }

        public string Name => "join";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack.Players.Any(player => player.Name == user.DiscordUsername))
            {
                return ("Ты уже принимаешь участие в игре");
            }
            if(Config.CurrentBlackJack.IsActive)
            {
                return "Раунд уже начался";
            }
            if(Config.CurrentBlackJack.Players.Count > 4)
            {
                return "Невозможно присоединиться, так как стол уже полон";
            }
            return Config.CurrentBlackJack.Join(user);
        }
    }
}