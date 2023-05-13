using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders
{
    public class JadeRodBuilder : ItemBuilder
    {
        public JadeRodBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
            IsActive = true;
        }

        public JadeRodBuilder WithRandomDistributedStats()
        {
            if (!IsRarityDefined())
            {
                throw new InvalidOperationException("Undefined item rarity");
            }

            var randomStats = RandomStatsDistributions[Rarity]();
            for (var i = 0; i < randomStats; i++)
            {
                switch (Randomizer.GetRandomNumberBetween(0, 2))
                {
                    case 0:
                        cooldownIncreasePercent += 20;
                        break;
                    case 1:
                        cooldownIncreaseTries++;
                        break;
                }
            }

            return this;
        }

        public JadeRod Build() => new()
        {
            Id = Id,
            Price = Price,
            Rarity = Rarity,
            IsActive = IsActive,
            Thickness = cooldownIncreasePercent,
            Length = cooldownIncreaseTries
        };

        private static readonly Dictionary<Rarity, Func<int>> RandomStatsDistributions = new()
        {
            { Rarity.Common, () => Randomizer.GetRandomNumberInclusive(1, 2) },
            { Rarity.Rare, () => Randomizer.GetRandomNumberInclusive(2, 3) },
            { Rarity.Epic, () => Randomizer.GetRandomNumberInclusive(3, 4) },
            { Rarity.Legendary, () => Randomizer.GetRandomNumberInclusive(4, 5) },
            { Rarity.BlackMarket, () => 6 }
        };

        private const int BaseCooldownIncreasePercent = 20;
        private const int BaseCooldownIncreaseTries = 1;

        private int cooldownIncreasePercent = BaseCooldownIncreasePercent;
        private int cooldownIncreaseTries = BaseCooldownIncreaseTries;
    }
}