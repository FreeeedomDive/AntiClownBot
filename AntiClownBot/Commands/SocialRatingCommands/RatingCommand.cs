using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class RatingCommand : BaseCommand
    {
        public RatingCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var member = await e.Guild.GetMemberAsync(user.DiscordId);
            var stringBuilder = new StringBuilder();

            var catWifeCount =
                $"{Utility.ItemToString(InventoryItem.CatWife)}: {user.UserItems[InventoryItem.CatWife]}";
            var dogWifeCount =
                $"{Utility.ItemToString(InventoryItem.DogWife)}: {user.UserItems[InventoryItem.DogWife]}";
            var riceBowlCount =
                $"{Utility.ItemToString(InventoryItem.RiceBowl)}: {user.UserItems[InventoryItem.RiceBowl]}";
            var gigabyteCount =
                $"{Utility.ItemToString(InventoryItem.Gigabyte)}: {user.UserItems[InventoryItem.Gigabyte]}";
            var jadeRodCount =
                $"{Utility.ItemToString(InventoryItem.JadeRod)}: {user.UserItems[InventoryItem.JadeRod]}";
            var communismPosterCount =
                $"{Utility.ItemToString(InventoryItem.CommunismPoster)}: {user.UserItems[InventoryItem.CommunismPoster]}";

            const int maxSpaceCount = 60;

            stringBuilder
                .Append($"Паспорт гражданин {member.Username}")
                .Append("\n")
                .Append($"Социальный рейтинг: {user.SocialRating}")
                .Append("\n")
                .Append(catWifeCount)
                .Append(" ".Repeat(maxSpaceCount - catWifeCount.Length))
                .Append(
                    $"{Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForCatWife, user.UserItems[InventoryItem.CatWife])}% шанс автоматический подношение")
                .Append("\n")
                .Append(dogWifeCount)
                .Append(" ".Repeat(maxSpaceCount - dogWifeCount.Length))
                .Append(
                    $"{Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForDogWife, user.UserItems[InventoryItem.DogWife])}% шанс уклониться от PIDOR")
                .Append("\n")
                .Append(riceBowlCount)
                .Append(" ".Repeat(maxSpaceCount - riceBowlCount.Length))
                .Append("диапазон получение рейтинг с подношение: ")
                .Append(
                    $"от {Constants.MinTributeValue - Constants.TributeDecreaseByOneRiceBowl * user.UserItems[InventoryItem.RiceBowl]} ")
                .Append(
                    $"до {Constants.MaxTributeValue + Constants.TributeIncreaseByOneRiceBowl * user.UserItems[InventoryItem.RiceBowl]}")
                .Append("\n")
                .Append(gigabyteCount)
                .Append(" ".Repeat(maxSpaceCount - gigabyteCount.Length))
                .Append(
                    $"{Constants.CooldownDecreaseChanceByOneGigabyte}% шанс срабатывания каждого гигабайт на понижение кулдаун на {(int) (Constants.CooldownDecreaseByOneGigabyteItem * 100)}%")
                .Append("\n")
                .Append(jadeRodCount)
                .Append(" ".Repeat(maxSpaceCount - jadeRodCount.Length))
                .Append(
                    $"{Constants.CooldownIncreaseChanceByOneJade}% шанс срабатывания каждого стержень на увеличение кулдаун в {Constants.CooldownIncreaseByOneJade} раза")
                .Append("\n")
                .Append(communismPosterCount)
                .Append(" ".Repeat(maxSpaceCount - communismPosterCount.Length))
                .Append(
                    $"{Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForCommunism, user.UserItems[InventoryItem.CommunismPoster])}% ")
                .Append("шанс разделить подношение пополам с кто-то другой");

            await e.Message.RespondAsync(stringBuilder.ToString());
        }

        public override string Help()
        {
            return "Получение своего социального рейтинга";
        }
    }
}