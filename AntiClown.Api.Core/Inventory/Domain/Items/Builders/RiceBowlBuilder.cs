using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders
{
    public class RiceBowlBuilder : ItemBuilder
    {
        public RiceBowlBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
        }

        public RiceBowlBuilder WithRandomTributeDecrease()
        {
            if (!IsRarityDefined())
            {
                throw new InvalidOperationException("Undefined item rarity");
            }

            tributeDecrease = TributeDecreaseGenerator[Rarity]();

            return this;
        }

        public RiceBowlBuilder WithRandomTributeIncrease()
        {
            if (!IsRarityDefined())
            {
                throw new InvalidOperationException("Undefined item rarity");
            }

            tributeIncrease = TributeIncreaseGenerator[Rarity]();

            return this;
        }

        public RiceBowlBuilder WithRandomDistributedStats()
        {
            if (!IsRarityDefined())
            {
                throw new InvalidOperationException("Undefined item rarity");
            }

            var randomStats = RandomStatsDistributions[Rarity];
            for (var i = 0; i < randomStats; i++)
            {
                switch (Randomizer.CoinFlip())
                {
                    case true:
                        tributeDecrease++;
                        break;
                    case false:
                        tributeIncrease++;
                        break;
                }
            }

            return this;
        }

        public RiceBowl Build() => new()
        {
            Id = Id,
            Price = Price,
            Rarity = Rarity,
            IsActive = false,
            NegativeRangeExtend = tributeDecrease,
            PositiveRangeExtend = tributeIncrease
        };

        private static readonly Dictionary<Rarity, Func<int>> TributeDecreaseGenerator = new()
        {
            { Rarity.Common, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(0, 9) },
            { Rarity.Rare, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(10, 19) },
            { Rarity.Epic, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(20, 24) },
            { Rarity.Legendary, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(25, 30) },
            { Rarity.BlackMarket, () => BaseTributeDecrease + Randomizer.GetRandomNumberBetweenIncludeRange(0, 30) }
        };

        private static readonly Dictionary<Rarity, Func<int>> TributeIncreaseGenerator = new()
        {
            { Rarity.Common, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(10, 24) },
            { Rarity.Rare, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(25, 39) },
            { Rarity.Epic, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(40, 54) },
            { Rarity.Legendary, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(55, 70) },
            { Rarity.BlackMarket, () => BaseTributeIncrease + Randomizer.GetRandomNumberBetweenIncludeRange(71, 100) }
        };

        private static readonly Dictionary<Rarity, int> RandomStatsDistributions = new()
        {
            { Rarity.Common, 2 },
            { Rarity.Rare, 4 },
            { Rarity.Epic, 6 },
            { Rarity.Legendary, 8 },
            { Rarity.BlackMarket, 10 }
        };

        private const int BaseTributeDecrease = 0;
        private const int BaseTributeIncrease = 0;

        private int tributeDecrease;
        private int tributeIncrease;
    }
}