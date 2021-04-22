using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Linq;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackHitCommand : BaseCommand
    {
        public BlackJackHitCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack == null)
            {
                await e.Message.RespondAsync("Стол не создан");
                return;
            }

            if (Config.CurrentBlackJack.Players.All(player => player.Name != user.DiscordUsername))
            {
                await e.Message.RespondAsync("Ты не принимаешь участие в игре");
                return;
            }

            if (!Config.CurrentBlackJack.IsActive)
            {
                await e.Message.RespondAsync("Раунд еще не начался");
                return;
            }

            if (Config.CurrentBlackJack.Players.Peek().Name != user.DiscordUsername)
            {
                await e.Message.RespondAsync("Не твой ход");
                return;
            }

            var result = Config.CurrentBlackJack.GetCard(false, Config.CurrentBlackJack.Players.Peek());
            Config.CurrentBlackJack.Players.Peek().DidHit = true;
            switch (result.Status)
            {
                case Models.BlackJack.GetResultStatus.Ok:
                    await e.Message.RespondAsync(result.Message);
                    break;
                case Models.BlackJack.GetResultStatus.NextPlayer:
                    var player = Config.CurrentBlackJack.Players.Dequeue();
                    Config.CurrentBlackJack.Players.Enqueue(player);
                    if (Config.CurrentBlackJack.Players.Peek().IsDealer)
                    {
                        await e.Message.RespondAsync(result.Message + Config.CurrentBlackJack.MakeResult());
                    }
                    else
                    {
                        await e.Message.RespondAsync(result.Message +
                                                     $"\n{Config.CurrentBlackJack.Players.Peek().Name}, твой ход");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Config.Save();
        }

        public override string Help()
        {
            return "Взятие еще одной карты у дилера";
        }
    }
}