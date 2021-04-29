using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Linq;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackStartCommand : BaseCommand
    {
        public BlackJackStartCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack == null)
            {
                await e.Message.RespondAsync("Стол не создан");
                return;
            }

            if (Config.CurrentBlackJack.IsActive)
            {
                await e.Message.RespondAsync("Раунд уже начался");
                return;
            }

            if (Config.CurrentBlackJack.Players.All(player => player.Name != user.DiscordUsername))
            {
                await e.Message.RespondAsync("Ты не принимаешь участие в игре");
                return;
            }

            await e.Message.RespondAsync(Config.CurrentBlackJack.StartRound());
            Config.Save();
        }

        public override string Help()
        {
            return "Начало нового раунда с присоединившимися игроками";
        }
    }
}