using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.GamblingCommands
{
    public class CloseGambleCommand: BaseCommand
    {
        public CloseGambleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            Config.CurrentGamble.CloseGamble();
            Config.Save();
            await e.Message.RespondAsync("Сборы закрыты");
        }

        public override string Help()
        {
            return "Остановка принятия ставок";
        }
    }
}