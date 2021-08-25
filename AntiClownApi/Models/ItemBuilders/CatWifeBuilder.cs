using System;
using System.Collections.Generic;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class CatWifeBuilder : ItemBuilder
    {
        private const int BaseChanceValue = 0;
        private int _autoTributeChance;

        public CatWifeBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
        }
        
        private static readonly Dictionary<Rarity, Func<int>> AutoTributeChanceGenerator = new()
        {
            {Rarity.Common, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(5, 8)},
            {Rarity.Rare, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(9, 12)},
            {Rarity.Epic, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(13, 16)},
            {Rarity.Legendary, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(17, 20)},
            {Rarity.BlackMarket, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(21, 25)}
        };

        public CatWifeBuilder WithRandomAutoTributeChance()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            _autoTributeChance = AutoTributeChanceGenerator[Rarity]();

            return this;
        }

        public CatWife Build() => new(Id)
        {
            Price = Price,
            Rarity = Rarity,
            AutoTributeChance = _autoTributeChance
        };

        public static CatWife CreateNew() =>
            new ItemBuilder()
                .WithRandomRarity()
                .WithPriceForSelectedRarity()
                .AsCatWife()
                .WithRandomAutoTributeChance()
                .Build();
    }
}