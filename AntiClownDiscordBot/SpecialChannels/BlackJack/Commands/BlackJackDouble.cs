using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Linq;

namespace AntiClownBot.SpecialChannels.BlackJack.Commands
{
    public class BlackJackDouble : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public BlackJackDouble(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "double";

        public string Execute(MessageCreateEventArgs e)
        {
            if (Config.CurrentBlackJack.Players.All(player => player.UserId != e.Author.Id))
            {
                return "Ты не принимаешь участие в игре";
            }

            if (!Config.CurrentBlackJack.IsActive)
            {
                return "Раунд еще не начат";
            }

            if (Config.CurrentBlackJack.Players.Peek().UserId != e.Author.Id)
            {
                return "Не твоя очередь";
            }

            if (Config.CurrentBlackJack.Players.Peek().DidHit)
            {
                return "Уже нельзя удваивать ставку";
            }
            Config.CurrentBlackJack.StopTimer();
            var result = Config.CurrentBlackJack.GetCard(true, Config.CurrentBlackJack.Players.Peek());
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
                                                     $"\n{Config.CurrentBlackJack.Players.Peek().Name}, твоя очередь");
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
