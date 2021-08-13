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

        public override Dictionary<string, string> ItemStatsForUser(SocialRatingUser user) => new()
        {
            {"Шанс на автоматическое подношение", $"{user.Stats.TributeAutoChance}%"}
        };

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateTributeAutoChance(user);
        }
    }
}