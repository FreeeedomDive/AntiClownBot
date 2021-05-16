using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Linq;

namespace AntiClownBot.SpecialChannels.BlackJack.Commands
{
    public class BlackJackHit : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public BlackJackHit(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "hit";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack.Players.All(player => player.Name != user.DiscordUsername))
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
            Config.CurrentBlackJack.StopTimer();
            var result = Config.CurrentBlackJack.GetCard(false, Config.CurrentBlackJack.Players.Peek());
            Config.CurrentBlackJack.Players.Peek().DidHit = true;
            switch (result.Status)
            {
                case Models.BlackJack.GetResultStatus.Ok:
                    Config.CurrentBlackJack.StartTimer();
                    return result.Message;
                case Models.BlackJack.GetResultStatus.NextPlayer:
                    var player = Config.CurrentBlackJack.Players.Dequeue();
                    Config.CurrentBlackJack.Players.Enqueue(player);
                    if (Config.CurrentBlackJack.Players.Peek().IsDealer)
                    {
                        return (result.Message + "\n" + Config.CurrentBlackJack.MakeResult());
                    }
                    else
                    {
                        Config.CurrentBlackJack.StartTimer();
                        return (result.Message +
                                                     $"\n{Config.CurrentBlackJack.Players.Peek().Name}, твой ход");
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
