using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class WhenCommand : BaseCommand
    {
        public WhenCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (e.Channel.Id != 877994939240292442)
            {
                await e.Message.RespondAsync($"{Utility.Emoji(":Madge:")} {Utility.Emoji(":point_right:")} {e.Guild.GetChannel(877994939240292442).Mention}");
                return;
            }
            if (user.IsCooldownPassed())
            {
                await e.Message.RespondAsync("Кулдаун уже прошел");
                return;
            }

            await e.Message.RespondAsync($"Следующий подношение император XI в {Utility.NormalizeTime(user.NextTribute)}, через {Utility.GetTimeDiff(user.NextTribute)}");
        }

        public override string Help()
        {
            return "Позволяет узнать время следующего подношения императору";
        }
    }
}