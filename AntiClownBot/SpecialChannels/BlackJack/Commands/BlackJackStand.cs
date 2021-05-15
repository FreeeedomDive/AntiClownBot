using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.BlackJack.Commands
{
    class BlackJackStand : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public BlackJackStand(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "stand";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack.Players.All(p => p.Name != user.DiscordUsername))
            {
                return "Ты не принимаешь участие в игре";
            }

            if (!Config.CurrentBlackJack.IsActive)
            {
                return "Раунд еще не начался";
            }

            if (Config.CurrentBlackJack.Players.Peek().Name != user.DiscordUsername)
            {
                return "Не твой ход";
            }

            var player = Config.CurrentBlackJack.Players.Dequeue();
            Config.CurrentBlackJack.Players.Enqueue(player);
            if (Config.CurrentBlackJack.Players.Peek().IsDealer)
            {
                return (Config.CurrentBlackJack.MakeResult());
            }

            return $"{Config.CurrentBlackJack.Players.Peek().Name}, твой ход";
        }
    }
}
