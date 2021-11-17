using System;
using System.Collections.Generic;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class JadeRodBuilder : ItemBuilder
    {
        private const int BaseCooldownIncreasePercent = 20;
        private const int BaseCooldownIncreaseTries = 1;

        private int _cooldownIncreasePercent = BaseCooldownIncreasePercent;
        private int _cooldownIncreaseTries = BaseCooldownIncreaseTries;

        private static readonly Dictionary<Rarity, Func<int>> RandomStatsDistributions = new()
        {
            {Rarity.Common, () => Randomizer.GetRandomNumberBetweenIncludeRange(1, 2)},
            {Rarity.Rare, () => Randomizer.GetRandomNumberBetweenIncludeRange(2, 3)},
            {Rarity.Epic, () => Randomizer.GetRandomNumberBetweenIncludeRange(3, 4)},
            {Rarity.Legendary, () => Randomizer.GetRandomNumberBetweenIncludeRange(4, 5)},
            {Rarity.BlackMarket, () => 6}
        };

        public JadeRodBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
            IsActive = true;
        }

        public JadeRodBuilder WithRandomDistributedStats()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            var randomStats = RandomStatsDistributions[Rarity]();
            for (var i = 0; i < randomStats; i++)
            {
                switch (Randomizer.GetRandomNumberBetween(0, 2))
                {
                    case 0:
                        _cooldownIncreasePercent += 20;
                        break;
                    case 1:
                        _cooldownIncreaseTries++;
                        break;
                }
            }

            return this;
        }

        public JadeRod Build() => new(Id)
        {
            Price = Price,
            Rarity = Rarity,
            IsActive = IsActive,
            Thickness = _cooldownIncreasePercent,
            Length = _cooldownIncreaseTries
        };
    }
}