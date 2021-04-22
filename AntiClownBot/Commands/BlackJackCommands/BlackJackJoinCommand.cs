using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Linq;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackJoinCommand : BaseCommand
    {
        public BlackJackJoinCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack == null)
            {
                await e.Message.RespondAsync("Стол не создан");
                return;
            }

            if (Config.CurrentBlackJack.Players.Any(player => player.Name == user.DiscordUsername))
            {
                await e.Message.RespondAsync("Ты уже принимаешь участие в игре");
                return;
            }

            if (Config.CurrentBlackJack.IsActive)
            {
                await e.Message.RespondAsync("Раунд уже начался");
                return;
            }

            await e.Message.RespondAsync(Config.CurrentBlackJack.Join(user));
            Config.Save();
        }

        public override string Help()
        {
            return "Принятие участия в игре, ставка 50";
        }
    }
}