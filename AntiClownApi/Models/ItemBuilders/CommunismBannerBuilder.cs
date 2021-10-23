using System;
using System.Collections.Generic;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class CommunismBannerBuilder : ItemBuilder
    {
        private const int BaseTributeDivideChance = 1;
        private const int BaseTributeStealChance = 1;

        private int _tributeDivideChance = BaseTributeDivideChance;
        private int _tributeStealChance = BaseTributeStealChance;

        private static readonly Dictionary<Rarity, Func<int>> RandomStatsDistributions = new()
        {
            {Rarity.Common, () => Randomizer.GetRandomNumberBetweenIncludeRange(3, 5)},
            {Rarity.Rare, () => Randomizer.GetRandomNumberBetweenIncludeRange(7, 9)},
            {Rarity.Epic, () => Randomizer.GetRandomNumberBetweenIncludeRange(11, 13)},
            {Rarity.Legendary, () => Randomizer.GetRandomNumberBetweenIncludeRange(15, 17)},
            {Rarity.BlackMarket, () => 20}
        };

        public CommunismBannerBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
            IsActive = true;
        }

        public CommunismBannerBuilder WithRandomDistributedStats()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            var randomStats = RandomStatsDistributions[Rarity]();
            for (var i = 0; i < randomStats; i++)
            {
                switch (Randomizer.GetRandomNumberBetween(0, 2))
                {
                    case 0:
                        _tributeDivideChance += 2;
                        break;
                    case 1:
                        _tributeStealChance++;
                        break;
                }
            }

            return this;
        }

        public CommunismBanner Build() => new(Id)
        {
            Price = Price,
            Rarity = Rarity,
            IsActive = IsActive,
            DivideChance = _tributeDivideChance,
            StealChance = _tributeStealChance
        };
    }
}