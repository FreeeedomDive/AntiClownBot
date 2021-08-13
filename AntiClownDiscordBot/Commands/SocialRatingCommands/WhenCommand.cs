using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class WhenCommand : BaseCommand
    {
        public WhenCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (user.IsCooldownPassed())
            {
                e.Message.RespondAsync("Кулдаун уже прошел");
                return;
            }

            e.Message.RespondAsync($"Следующий подношение император XI в {Utility.NormalizeTime(user.NextTribute)}, через {Utility.GetTimeDiff(user.NextTribute)}");
        }

        public override string Help()
        {
            return "Позволяет узнать время следующего подношения императору";
        }
    }
}