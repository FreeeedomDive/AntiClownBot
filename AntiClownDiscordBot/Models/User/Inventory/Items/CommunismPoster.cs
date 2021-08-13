using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory.Items
{
    public class CommunismPoster : Item
    {
        public override string Name => "Коммунистический плакат";
        public override int Price => -1500;

        public override Dictionary<string, string> ItemStatsForUser(SocialRatingUser user)
        {
            var dict = new Dictionary<string, string>
            {
                {
                    "Шанс разделить награда за подношение с другим владелец Коммунистический плакат",
                    $"{user.Stats.TributeSplitChance}%"
                },
                {
                    "Приблизительный шанс получить разделение чужой подношение",
                    $"{((Utility.GetCommunistsDictionary().TryGetValue(user, out var count) ? count : 0) * 100) / Utility.GetDistributedCommunists().Count}%\n(может меняться в зависимости от владельца подношения)"
                }
            };


            return dict;
        }

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateTributeSplitChance(user);
        }
    }
}