using AntiClownBot.Helpers;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class SocialStatusCommand: BaseCommand
    {
        public SocialStatusCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            await e.Message.RespondAsync($"{Utility.Emoji(":NOPERS:")} {Utility.Emoji(":NOPERS:")} {Utility.Emoji(":NOPERS:")}");
        }

        public override string Help()
        {
            return "Рейтинг пользователей по социальному статусу";
        }
    }
}