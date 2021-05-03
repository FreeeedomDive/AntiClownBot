using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class SocialStatusCommand: BaseCommand
    {
        public SocialStatusCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            await e.Message.RespondAsync(Config.GetSocialRatingStats());
        }

        public override string Help()
        {
            return "Рейтинг пользователей по социальному статусу";
        }
    }
}