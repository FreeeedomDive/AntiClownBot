using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory.Items
{
    public class DogWife : Item
    {
        public override string Name => "Собака-жена";
        public override int Price => 1000;
        public override string Perks => "Шанс уклониться от 5 букв";

        public override string ItemStatsForUser(SocialRatingUser user) => $"{user.Stats.PidorEvadeChance}%";

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculatePidorEvadeChance(user);
        }
    }
}