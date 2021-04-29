using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Linq;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackDoubleCommand : BaseCommand
    {
        public BlackJackDoubleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
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
                await e.Message.RespondAsync("Раунд еще не начат");
                return;
            }

            if (Config.CurrentBlackJack.Players.Peek().Name != user.DiscordUsername)
            {
                await e.Message.RespondAsync("Не твоя очередь");
                return;
            }

            if (Config.CurrentBlackJack.Players.Peek().DidHit)
            {
                await e.Message.RespondAsync("Уже нельзя удваивать ставку");
                return;
            }

            var result = Config.CurrentBlackJack.GetCard(true, Config.CurrentBlackJack.Players.Peek());
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
                        await e.Message.RespondAsync(result.Message + "\n" + Config.CurrentBlackJack.MakeResult());
                    }
                    else
                    {
                        await e.Message.RespondAsync(result.Message +
                                                     $"\n{Config.CurrentBlackJack.Players.Peek().Name}, твоя очередь");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Config.Save();
        }

        public override string Help() =>
            "Игроку можно удвоить ставку, но он обязан остановиться после этого взятия карты.\nПосле того, как игрок уже брал карты, удваивать больше нельзя";
    }
}