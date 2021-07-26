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
        public override string Perks => "Шанс уменьшить подготовка подношение";

        public override string ItemStatsForUser(SocialRatingUser user) =>
            $"{Constants.CooldownDecreaseChanceByOneGigabyte + user.Stats.CooldownDecreaseChanceExtend}% " +
            $"{user.Stats.CooldownDecreaseTryCount} раз на {(int) (Constants.CooldownDecreaseByOneGigabyteItem * 100)}%";

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateCooldownDecreaseTryCount(user);
        }
    }
}