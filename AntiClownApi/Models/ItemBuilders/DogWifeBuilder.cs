using System;
using System.Collections.Generic;
using AntiClownBotApi.Models.Classes.Items;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class DogWifeBuilder: ItemBuilder
    {
        private const int BaseLootBoxFindChance = 10;

        private int _lootBoxFindChance;

        public DogWifeBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
        }
        
        private static readonly Dictionary<Rarity, Func<int>> LootBoxFindChanceDistribution = new()
        {
            {Rarity.Common, () => BaseLootBoxFindChance + 0},
            {Rarity.Rare, () => BaseLootBoxFindChance + Randomizer.GetRandomNumberBetweenIncludeRange(1, 2)},
            {Rarity.Epic, () => BaseLootBoxFindChance + Randomizer.GetRandomNumberBetweenIncludeRange(3, 4)},
            {Rarity.Legendary, () => BaseLootBoxFindChance + Randomizer.GetRandomNumberBetweenIncludeRange(5, 7)},
            {Rarity.BlackMarket, () => BaseLootBoxFindChance + Randomizer.GetRandomNumberBetweenIncludeRange(8, 10)}
        };

        public DogWifeBuilder WithRandomLootBoxFindChance()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            _lootBoxFindChance = LootBoxFindChanceDistribution[Rarity]();

            return this;
        }

        public DogWife Build() => new DogWife(Id)
        {
            Price = Price,
            Rarity = Rarity,
            LootBoxFindChance = _lootBoxFindChance
        };
    }
}