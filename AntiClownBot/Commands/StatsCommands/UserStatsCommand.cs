using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace AntiClownBot.Commands.StatsCommands
{
    public class UserStatsCommand : BaseCommand
    {
        public UserStatsCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var member = await e.Guild.GetMemberAsync(user.DiscordId);


            var stringBuilder = new StringBuilder();
            stringBuilder
                .Append($"```Статы гражданин {member.Username}")
                .Append("\n")
                .Append($"Шанс на автоматическое подношение: {user.Stats.TributeAutoChance}%\n")
                .Append($"Шанс уклониться от 5 букв: {user.Stats.PidorEvadeChance}%\n")
                .Append(
                    $"Подношение от {Constants.MinTributeValue - user.Stats.TributeLowerExtendBorder} до {Constants.MaxTributeValue + user.Stats.TributeUpperExtendBorder}\n")
                .Append($"Шанс уменьшить подготовка подношение: {Constants.CooldownDecreaseChanceByOneGigabyte + user.Stats.CooldownDecreaseChanceExtend}% {user.Stats.CooldownDecreaseTryCount} раз на {(int)(Constants.CooldownDecreaseByOneGigabyteItem*100)}%\n")
                .Append($"Шанс увеличить подготовка подношение: {Constants.CooldownIncreaseChanceByOneJade + user.Stats.CooldownIncreaseChanceExtend}% {user.Stats.CooldownIncreaseTryCount} раз в {Constants.CooldownIncreaseByOneJade} раз\n")
                .Append($"Шанс разделить награда за подношение с другим владелец Коммунистический плакат: {user.Stats.TributeSplitChance}%")
                .Append("```");
            await e.Message.RespondAsync(stringBuilder.ToString());
        }

        public override string Help()
        {
            return "Показывает статы гражданина";
        }
    }
}
