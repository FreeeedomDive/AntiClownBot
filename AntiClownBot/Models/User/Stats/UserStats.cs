using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiClownBot.Models.User.Inventory.Items;

namespace AntiClownBot.Models.User.Stats
{
    public class UserStats
    {
        public int TributeUpperExtendBorder { get; private set; }
        public int TributeLowerExtendBorder { get; private set; }
        public int TributeAutoChance { get; private set; }
        public int PidorEvadeChance { get; private set; }
        public int CooldownDecreaseChanceExtend { get; private set; }
        public int CooldownDecreaseTryCount { get; private set; }
        public int CooldownIncreaseChanceExtend { get; private set; }
        public int CooldownIncreaseTryCount { get; private set; }
        public int TributeSplitChance { get; private set; }

        public void RecalculateTributeBorders(SocialRatingUser user)
        {
            TributeUpperExtendBorder = user.Items[new RiceBowl()]*5;
            TributeLowerExtendBorder = user.Items[new RiceBowl()]*2;
        }

        public void RecalculateTributeAutoChance(SocialRatingUser user)
        {
            TributeAutoChance = Utility.LogarithmicDistribution(16, user.Items[new CatWife()]);
        }

        public void RecalculatePidorEvadeChance(SocialRatingUser user)
        {
            PidorEvadeChance = Utility.LogarithmicDistribution(16, user.Items[new DogWife()]);
        }

        public void RecalculateCooldownDecreaseTryCount(SocialRatingUser user)
        {
            CooldownDecreaseTryCount = user.Items[new Gigabyte()];
        }

        public void RecalculateCooldownIncreaseTryCount(SocialRatingUser user)
        {
            CooldownIncreaseTryCount = user.Items[new JadeRod()];
        }

        public void RecalculateTributeSplitChance(SocialRatingUser user)
        {
            TributeSplitChance = Utility.LogarithmicDistribution(4, user.Items[new CommunismPoster()]);
        }

        public void RecalculateAllStats(SocialRatingUser user)
        {
            RecalculateTributeBorders(user);
            RecalculateTributeAutoChance(user);
            RecalculatePidorEvadeChance(user);
            RecalculateCooldownDecreaseTryCount(user);
            RecalculateCooldownIncreaseTryCount(user);
            RecalculateTributeSplitChance(user);
        }
    }
}
