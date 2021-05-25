using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory.Items
{
    public class RiceBowl : Item
    {
        public override string Name => "Рис миска";
        public override int Price => 1000;
        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateTributeBorders(user);
        }
    }
}