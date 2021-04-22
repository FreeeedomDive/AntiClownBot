using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackEndCommand : BaseCommand
    {
        public BlackJackEndCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
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
                await e.Message.RespondAsync("Невозможно закрыть текущий стол, так как раунд еще не закончен");
                return;
            }

            Config.CurrentBlackJack = null;
            await e.Message.RespondAsync("Стол закрыт");
            Config.Save();
        }

        public override string Help()
        {
            return $"Завершение игры {Utility.StringEmoji(":5Head:")}";
        }
    }
}