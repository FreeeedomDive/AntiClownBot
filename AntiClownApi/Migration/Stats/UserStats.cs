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
        public int ExtraTributeAutoChance { get; private set; }
        public int PidorEvadeChance { get; private set; }
        public int ExtraPidorEvadeChance { get; private set; }
        public int CooldownDecreaseChanceExtend { get; private set; }
        public int CooldownDecreaseTryCount { get; private set; }
        public int CooldownIncreaseChanceExtend { get; private set; }
        public int CooldownIncreaseTryCount { get; private set; }
        public int TributeSplitChance { get; private set; }
        public int ExtraTributeSplitChance { get; private set; }
        public int Luck { get; private set; }

        private readonly int[] _standardChanceDistribution = new[] {8, 8, 4, 4, 4};

        private void RecalculateLuck()
        {
            Luck = ExtraTributeAutoChance + ExtraPidorEvadeChance - ExtraTributeSplitChance;
        }
        public void RecalculateTributeBorders(SocialRatingUser user)
        {
            TributeUpperExtendBorder = user.Items[new RiceBowl()]*5;
            TributeLowerExtendBorder = user.Items[new RiceBowl()]*2;
        }

        public void RecalculateTributeAutoChance(SocialRatingUser user)
        {
            TributeAutoChance = 0;
            var count = user.Items[new CatWife()];
            var distrCount = 0;
            while (distrCount < _standardChanceDistribution.Length && count > 0)
            {
                TributeAutoChance += _standardChanceDistribution[distrCount];
                distrCount++;
                count--;
            }

            TributeAutoChance += count * 2;
            ExtraTributeAutoChance = Math.Max(0, TributeAutoChance - 60);
            RecalculateLuck();
        }

        public void RecalculatePidorEvadeChance(SocialRatingUser user)
        {
            PidorEvadeChance = 0;
            var count = user.Items[new DogWife()];
            var distrCount = 0;
            while (distrCount < _standardChanceDistribution.Length && count > 0)
            {
                PidorEvadeChance += _standardChanceDistribution[distrCount];
                distrCount++;
                count--;
            }

            PidorEvadeChance += count * 2;
            if (PidorEvadeChance > 60)
            {
                ExtraPidorEvadeChance = PidorEvadeChance - 60;
                PidorEvadeChance = 60;
            }
            else
            {
                ExtraPidorEvadeChance = 0;
            }
            RecalculateLuck();
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
            TributeSplitChance = 0;
            var count = user.Items[new CommunismPoster()];
            TributeSplitChance += count * 2;
            if (TributeSplitChance > 60)
            {
                ExtraTributeSplitChance = TributeSplitChance - 60;
                TributeSplitChance = 60;
            }
            else
            {
                ExtraTributeSplitChance = 0;
            }
            RecalculateLuck();
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
