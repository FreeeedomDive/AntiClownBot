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
        public override int Price => -4000;
        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateTributeSplitChance(user);
        }
    }
}