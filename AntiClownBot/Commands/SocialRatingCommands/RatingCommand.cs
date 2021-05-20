using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntiClownBot.Models.User.Inventory.Items;
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

//             var r1 = user.UserItems[InventoryItem.CatWife];
//             var r2 = user.UserItems[InventoryItem.DogWife];
//             var r3 = user.UserItems[InventoryItem.RiceBowl];
//             var r4 = user.UserItems[InventoryItem.Gigabyte];
//             var r5 = user.UserItems[InventoryItem.JadeRod];
//             var r6 = user.UserItems[InventoryItem.CommunismPoster];
// 
//             var result =
//                 @$"```text
// Паспорт гражданин {member.Username}
// Социальный рейтинг: {user.SocialRating}
// кошка-жена: {r1}                 - {Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForCatWife, user.UserItems[InventoryItem.CatWife])}% шанс автоматический подношение
// собака-жена: {r2}                - {Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForDogWife, user.UserItems[InventoryItem.DogWife])}% шанс уклониться от PIDOR
// рис миска: {r3}                  - диапазон получение рейтинг с подношение: от {Constants.MinTributeValue - Constants.TributeDecreaseByOneRiceBowl * user.UserItems[InventoryItem.RiceBowl]} до {Constants.MaxTributeValue + Constants.TributeIncreaseByOneRiceBowl * user.UserItems[InventoryItem.RiceBowl]}
// гигабайт интернет: {r4}          - {Constants.CooldownDecreaseChanceByOneGigabyte}% шанс срабатывания каждого гигабайт на понижение кулдаун на {(int) (Constants.CooldownDecreaseByOneGigabyteItem * 100)}%
// нефритовый стержень: {r5}        - {Constants.CooldownIncreaseChanceByOneJade}% шанс срабатывания каждого стержень на увеличение кулдаун в {Constants.CooldownIncreaseByOneJade} раза
// коммунистический плакат: {r6}    - {Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForCommunism, user.UserItems[InventoryItem.CommunismPoster])}% шанс разделить подношение пополам с кто-то другой
// ```";


            var stringBuilder = new StringBuilder();
            var catWifeCount =
                $"{CatWife.Name}: {user.Items[new CatWife()]}";
            var dogWifeCount =
                $"{DogWife.Name}: {user.Items[new DogWife()]}";
            var riceBowlCount =
                $"{RiceBowl.Name}: {user.Items[new RiceBowl()]}";
            var gigabyteCount =
                $"{Gigabyte.Name}: {user.Items[new Gigabyte()]}";
            var jadeRodCount =
                $"{JadeRod.Name}: {user.Items[new JadeRod()]}";
            var communismPosterCount =
                $"{CommunismPoster.Name}: {user.Items[new CommunismPoster()]}";
            const int maxSpaceCount = 35;
            stringBuilder
                .Append($"```Паспорт гражданин {member.Username}")
                .Append("\n")
                .Append($"Социальный рейтинг: {user.SocialRating}")
                .Append("\n")
                .Append($"Общий рейтинг: {user.NetWorth}")
                .Append("\n")
                .Append(catWifeCount);
            if (user.Items[new CatWife()] != 0)
            {
                stringBuilder
                    .Append(" ".Repeat(maxSpaceCount - catWifeCount.Length))
                    .Append(
                        $"{Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForCatWife, user.Items[new CatWife()])}% шанс автоматический подношение");
            }

            stringBuilder
                .Append("\n")
                .Append(dogWifeCount);

            if (user.Items[new DogWife()] != 0)
            {
                stringBuilder
                    .Append(" ".Repeat(maxSpaceCount - dogWifeCount.Length))
                    .Append(
                        $"{Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForDogWife, user.Items[new DogWife()])}% шанс уклониться от PIDOR");
            }

            stringBuilder
                .Append("\n")
                .Append(riceBowlCount);

            if (user.Items[new RiceBowl()] != 0)
            {
                stringBuilder
                    .Append(" ".Repeat(maxSpaceCount - riceBowlCount.Length))
                    .Append("диапазон получение рейтинг с подношение: ")
                    .Append(
                        $"от {Constants.MinTributeValue - Constants.TributeDecreaseByOneRiceBowl * user.Items[new RiceBowl()]} ")
                    .Append(
                        $"до {Constants.MaxTributeValue + Constants.TributeIncreaseByOneRiceBowl * user.Items[new RiceBowl()]}");
            }

            stringBuilder
                .Append("\n")
                .Append(gigabyteCount);

            if (user.Items[new Gigabyte()] != 0)
            {
                stringBuilder
                    .Append(" ".Repeat(maxSpaceCount - gigabyteCount.Length))
                    .Append(
                        $"{Constants.CooldownDecreaseChanceByOneGigabyte}% шанс срабатывания каждого гигабайт на понижение кулдаун на {(int) (Constants.CooldownDecreaseByOneGigabyteItem * 100)}%");
            }

            stringBuilder
                .Append("\n")
                .Append(jadeRodCount);

            if (user.Items[new JadeRod()] != 0)
            {
                stringBuilder
                    .Append(" ".Repeat(maxSpaceCount - jadeRodCount.Length))
                    .Append(
                        $"{Constants.CooldownIncreaseChanceByOneJade}% шанс срабатывания каждого стержень на увеличение кулдаун в {Constants.CooldownIncreaseByOneJade} раза");
            }

            stringBuilder
                .Append("\n")
                .Append(communismPosterCount);

            if (user.Items[new CommunismPoster()] != 0)
            {
                stringBuilder
                    .Append(" ".Repeat(maxSpaceCount - communismPosterCount.Length))
                    .Append(
                        $"{Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForCommunism, user.Items[new CommunismPoster()])}% ")
                    .Append("шанс разделить подношение пополам с кто-то другой");
            }

            stringBuilder
                .Append("\n```");

            await e.Message.RespondAsync(stringBuilder.ToString());
        }

        public override string Help()
        {
            return "Получение своего социального рейтинга";
        }
    }
}