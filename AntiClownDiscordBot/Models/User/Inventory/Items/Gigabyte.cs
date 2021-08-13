using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory.Items
{
    public class Gigabyte : Item
    {
        public override string Name => "Гигабайт интернет";
        public override int Price => 1000;

        public override Dictionary<string, string> ItemStatsForUser(SocialRatingUser user) => new()
        {
            {
                "Шанс уменьшить подготовка подношение",
                $"{Constants.CooldownDecreaseChanceByOneGigabyte + user.Stats.CooldownDecreaseChanceExtend}% " +
                $"{user.Stats.CooldownDecreaseTryCount} раз на {(int) (Constants.CooldownDecreaseByOneGigabyteItem * 100)}%"
            },
            {
                "Шанс срабатывания хотя бы одного гигабайта",
                $"≈{(int)((1 - Math.Pow(1 - 0.01 * Constants.CooldownDecreaseChanceByOneGigabyte, user.Items[this])) * 100)}%"
            }
        };

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateCooldownDecreaseTryCount(user);
        }
    }
}