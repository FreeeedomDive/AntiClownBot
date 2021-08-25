using System;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class WhenCommand : BaseCommand
    {
        public WhenCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            if (e.Channel.Id != 877994939240292442 && e.Channel.Id != 879784704696549498)
            {
                await e.Message.RespondAsync($"{Utility.Emoji(":Madge:")} {Utility.Emoji(":point_right:")} {e.Guild.GetChannel(877994939240292442).Mention}");
                return;
            }

            var result = ApiWrapper.Wrappers.UsersApi.WhenNextTribute(e.Author.Id);
            var now = DateTime.Now;
            var cooldownHasPassed = now > result.NextTribute;
            
            if (cooldownHasPassed)
            {
                await e.Message.RespondAsync("Кулдаун уже прошел");
                return;
            }

            await e.Message.RespondAsync($"Следующий подношение император XI в {Utility.NormalizeTime(result.NextTribute)}, через {Utility.GetTimeDiff(result.NextTribute)}");
        }

        public override string Help()
        {
            return "Позволяет узнать время следующего подношения императору";
        }
    }
}