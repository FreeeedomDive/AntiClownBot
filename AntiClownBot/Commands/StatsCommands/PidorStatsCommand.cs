using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.StatsCommands
{
    public class PidorStatsCommand: BaseCommand
    {
        public PidorStatsCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            await e.Message.RespondAsync(Config.GetPidorStats());
        }

        public override string Help()
        {
            return "Статистика получения пидора на сервере";
        }
    }
}