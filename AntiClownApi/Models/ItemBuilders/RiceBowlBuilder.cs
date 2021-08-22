using System;
using System.Collections.Generic;
using AntiClownBotApi.Models.Classes.Items;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class RiceBowlBuilder : ItemBuilder
    {
        private const int BaseTributeDecrease = 0;
        private const int BaseTributeIncrease = 0;

        private int _tributeDecrease;
        private int _tributeIncrease;

        public RiceBowlBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
        }
        
        private static readonly Dictionary<Rarity, Func<int>> TributeDecreaseGenerator = new()
        {
            {Rarity.Common, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(0, 9)},
            {Rarity.Rare, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(10, 19)},
            {Rarity.Epic, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(20, 24)},
            {Rarity.Legendary, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(25, 30)},
            {Rarity.BlackMarket, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(0, 30)}
        };

        private static readonly Dictionary<Rarity, Func<int>> TributeIncreaseGenerator = new()
        {
            {Rarity.Common, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(10, 24)},
            {Rarity.Rare, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(25, 39)},
            {Rarity.Epic, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(40, 54)},
            {Rarity.Legendary, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(55, 70)},
            {Rarity.BlackMarket, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(71, 100)}
        };

        private static readonly Dictionary<Rarity, int> RandomStatsDistributions = new()
        {
            {Rarity.Common, 2},
            {Rarity.Rare, 4},
            {Rarity.Epic, 6},
            {Rarity.Legendary, 8},
            {Rarity.BlackMarket, 10}
        };

        public RiceBowlBuilder WithRandomTributeDecrease()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            _tributeDecrease = TributeDecreaseGenerator[Rarity]();

            return this;
        }
        
        public RiceBowlBuilder WithRandomTributeIncrease()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            _tributeIncrease = TributeIncreaseGenerator[Rarity]();

            return this;
        }

        public RiceBowlBuilder WithRandomDistributedStats()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");
            
            var randomStats = RandomStatsDistributions[Rarity];
            for (var i = 0; i < randomStats; i++)
            {
                switch (Randomizer.GetRandomNumberBetween(0, 2))
                {
                    case 0:
                        _tributeDecrease++;
                        break;
                    case 1:
                        _tributeIncrease++;
                        break;
                }
            }
            
            return this;
        }

        public RiceBowl Build() => new(Id)
        {
            Price = Price,
            Rarity = Rarity,
            NegativeRangeExtend = _tributeDecrease,
            PositiveRangeExtend = _tributeIncrease
        };
    }
}