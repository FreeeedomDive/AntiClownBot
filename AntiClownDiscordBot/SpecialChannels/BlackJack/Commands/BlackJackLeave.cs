using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.BlackJack.Commands
{
    public class BlackJackLeave : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public BlackJackLeave(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "leave";

        public string Execute(MessageCreateEventArgs e)
        {
            if (Config.CurrentBlackJack.Players.All(player => player.UserId != e.Author.Id))
            {
                return "Ты не принимаешь участие в игре";
            }
            if(Config.CurrentBlackJack.Players.First().UserId == e.Author.Id)
            {
                Config.CurrentBlackJack.StopTimer();
            }
            var message = (Config.CurrentBlackJack.Leave(e.Author.Id));
            if (Config.CurrentBlackJack.Players.First().IsDealer && Config.CurrentBlackJack.IsActive)
            {
                Config.CurrentBlackJack.StopTimer();
                message += Config.CurrentBlackJack.MakeResult();
            }
            else if (Config.CurrentBlackJack.IsActive)
            {
                message += $"{Config.CurrentBlackJack.Players.First().Name}, твоя очередь";
            }
            return message;
        }
    }
}
