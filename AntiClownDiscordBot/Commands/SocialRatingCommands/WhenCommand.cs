using System;
using AntiClownBot.Helpers;
using DSharpPlus;
using DSharpPlus.Entities;
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
                await e.Message.RespondAsync(
                    $"{Utility.Emoji(":Madge:")} {Utility.Emoji(":point_right:")} {e.Guild.GetChannel(877994939240292442).Mention}");
                return;
            }

            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle("А когда же подношение???");

            var result = ApiWrapper.Wrappers.UsersApi.WhenNextTribute(e.Author.Id);
            var cooldownHasPassed = DateTime.Now > result.NextTribute;

            if (cooldownHasPassed)
            {
                embedBuilder.WithColor(DiscordColor.Green);
                embedBuilder.AddField("Уже пора!!!",
                    "Срочно нужно исполнить партийный долг " + Utility.Emoji(":flag_cn:").ToString().Multiply(3));
                embedBuilder.AddField($"А мог бы прийти и пораньше {Utility.Emoji(":Clueless:")}",
                    $"Ты опоздал на {Utility.GetTimeDiff(result.NextTribute)}");
                await e.Message.RespondAsync(embedBuilder.Build());
                return;
            }

            embedBuilder.WithColor(DiscordColor.Red);
            var roflan = Randomizer.GetRandomNumberBetween(0, 100);
            if (roflan == 69)
            {
                embedBuilder.AddField("Завтра в 3", $"{Utility.Emoji(":aRolf:")}".Multiply(5));
            }
            else
            {
                embedBuilder.AddField($"А подношение император XI через {Utility.GetTimeDiff(result.NextTribute)}",
                    $"Приходи не раньше чем {Utility.NormalizeTime(result.NextTribute)}");
            }

            await e.Message.RespondAsync(embedBuilder.Build());
        }

        public override string Help()
        {
            return "Позволяет узнать время следующего подношения императору";
        }
    }
}