using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory.Items
{
    public class CatWife : Item
    {
        public override string Name => "Кошка-жена";
        public override int Price => 1000;
        public override string Perks => "Шанс на автоматическое подношение";

        public override string ItemStatsForUser(SocialRatingUser user) => $"{user.Stats.TributeAutoChance}%";

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateTributeAutoChance(user);
        }
    }
}
