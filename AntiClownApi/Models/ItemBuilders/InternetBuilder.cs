using System;
using System.Collections.Generic;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class InternetBuilder : ItemBuilder
    {
        private const int BaseCooldownReducePercent = 8;
        private const int BaseCooldownReduceTries = 1;
        private const int BaseCooldownReduceChance = 3;

        private int _cooldownReducePercent;
        private int _cooldownReduceTries;
        private int _cooldownReduceChance;

        public InternetBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
        }

        private static readonly Dictionary<Rarity, int> CooldownReducePercentDistribution = new()
        {
            {Rarity.Common, BaseCooldownReducePercent + 0},
            {Rarity.Rare, BaseCooldownReducePercent + 1},
            {Rarity.Epic, BaseCooldownReducePercent + 2},
            {Rarity.Legendary, BaseCooldownReducePercent + 3},
            {Rarity.BlackMarket, BaseCooldownReducePercent + 4}
        };

        private static readonly Dictionary<Rarity, int> CooldownReduceTriesDistribution = new()
        {
            {Rarity.Common, BaseCooldownReduceTries + 0},
            {Rarity.Rare, BaseCooldownReduceTries + 1},
            {Rarity.Epic, BaseCooldownReduceTries + 2},
            {Rarity.Legendary, BaseCooldownReduceTries + 3},
            {Rarity.BlackMarket, BaseCooldownReduceTries + 4}
        };


        private static readonly Dictionary<Rarity, int> CooldownReduceChanceDistribution = new()
        {
            {Rarity.Common, BaseCooldownReduceChance + 0},
            {Rarity.Rare, BaseCooldownReduceChance + 1},
            {Rarity.Epic, BaseCooldownReduceChance + 2},
            {Rarity.Legendary, BaseCooldownReduceChance + 3},
            {Rarity.BlackMarket, BaseCooldownReduceChance + 4}
        };

        public static readonly Dictionary<Rarity, Func<int>> RandomStatsDistributions = new()
        {
            {Rarity.Common, () => Randomizer.GetRandomNumberBetweenIncludeRange(1, 3)},
            {Rarity.Rare, () => Randomizer.GetRandomNumberBetweenIncludeRange(1, 4)},
            {Rarity.Epic, () => Randomizer.GetRandomNumberBetweenIncludeRange(2, 4)},
            {Rarity.Legendary, () => Randomizer.GetRandomNumberBetweenIncludeRange(2, 5)},
            {Rarity.BlackMarket, () => Randomizer.GetRandomNumberBetweenIncludeRange(3, 5)}
        };

        public InternetBuilder WithRandomCooldownReducePercent()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            _cooldownReducePercent = CooldownReducePercentDistribution[Rarity];

            return this;
        }

        public InternetBuilder WithRandomCooldownReduceTries()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            _cooldownReduceTries = CooldownReduceTriesDistribution[Rarity];

            return this;
        }

        public InternetBuilder WithRandomCooldownReduceChance()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            _cooldownReduceChance = CooldownReduceChanceDistribution[Rarity];

            return this;
        }

        public InternetBuilder WithRandomDistributedStats()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            var randomStats = RandomStatsDistributions[Rarity]();
            for (var i = 0; i < randomStats; i++)
            {
                switch (Randomizer.GetRandomNumberBetween(0, 2))
                {
                    case 0:
                        _cooldownReducePercent++;
                        break;
                    case 1:
                        _cooldownReduceTries++;
                        break;
                    case 2:
                        _cooldownReduceChance++;
                        break;
                }
            }

            return this;
        }

        public Internet Build() => new Internet(Id)
        {
            Price = Price,
            Rarity = Rarity,
            Speed = _cooldownReducePercent,
            Gigabytes = _cooldownReduceTries,
            Ping = _cooldownReduceChance
        };
    }
}