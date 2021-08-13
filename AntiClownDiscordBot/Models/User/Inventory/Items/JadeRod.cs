using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory.Items
{
    public class JadeRod : Item
    {
        public override string Name => "Нефритовый стержень";
        public override int Price => -1500;

        public override Dictionary<string, string> ItemStatsForUser(SocialRatingUser user) => new()
        {
            {
                "Шанс увеличить подготовка подношение",
                $"{Constants.CooldownIncreaseChanceByOneJade + user.Stats.CooldownIncreaseChanceExtend}% " +
                $"{user.Stats.CooldownIncreaseTryCount} раз в {Constants.CooldownIncreaseByOneJade} раз"
            },
            {
                "Шанс несрабатывания ни одного нефритового стержня",
                $"≈ {(int) (Math.Pow(1 - 0.01 * Constants.CooldownIncreaseChanceByOneJade, user.Items[this]) * 100)}%"
            }
        };

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateCooldownIncreaseTryCount(user);
        }
    }
}