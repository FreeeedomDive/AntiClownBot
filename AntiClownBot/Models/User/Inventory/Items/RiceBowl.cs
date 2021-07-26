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
        public override string Perks => "Получение рейтинг с подношения";

        public override string ItemStatsForUser(SocialRatingUser user) =>
            $"от {Constants.MinTributeValue - user.Stats.TributeLowerExtendBorder} " +
            $"до {Constants.MaxTributeValue + user.Stats.TributeUpperExtendBorder}";

        public override void OnItemAddOrRemove(SocialRatingUser user)
        {
            user.Stats.RecalculateTributeBorders(user);
        }
    }
}