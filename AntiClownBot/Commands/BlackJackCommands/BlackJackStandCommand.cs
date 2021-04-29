using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Linq;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackStandCommand : BaseCommand
    {
        public BlackJackStandCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack == null)
            {
                await e.Message.RespondAsync("Стол не создан");
                return;
            }

            if (Config.CurrentBlackJack.Players.All(p => p.Name != user.DiscordUsername))
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

            var player = Config.CurrentBlackJack.Players.Dequeue();
            Config.CurrentBlackJack.Players.Enqueue(player);
            if (Config.CurrentBlackJack.Players.Peek().IsDealer)
            {
                await e.Message.RespondAsync(Config.CurrentBlackJack.MakeResult());
                Config.Save();
                return;
            }

            await e.Message.RespondAsync($"{Config.CurrentBlackJack.Players.Peek().Name}, твой ход");
            Config.Save();
        }

        public override string Help()
        {
            return "Остановка взятия новых карт";
        }
    }
}