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
        public override string Perks => "Шанс увеличить подготовка подношение";

        public override string ItemStatsForUser(SocialRatingUser user) =>
            $"{Constants.CooldownIncreaseChanceByOneJade + user.Stats.CooldownIncreaseChanceExtend}% " +
            $"{user.Stats.CooldownIncreaseTryCount} раз в {Constants.CooldownIncreaseByOneJade} раз";

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateCooldownIncreaseTryCount(user);
        }
    }
}