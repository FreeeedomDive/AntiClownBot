using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders
{
    public class DogWifeBuilder : ItemBuilder
    {
        public DogWifeBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
        }

        public DogWifeBuilder WithRandomLootBoxFindChance()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            lootBoxFindChance = LootBoxFindChanceDistribution[Rarity]();

            return this;
        }

        public DogWife Build() => new()
        {
            Id = Id,
            Price = Price,
            Rarity = Rarity,
            IsActive = false,
            LootBoxFindChance = lootBoxFindChance
        };

        private static readonly Dictionary<Rarity, Func<int>> LootBoxFindChanceDistribution = new()
        {
            { Rarity.Common, () => BaseLootBoxFindChance + 0 },
            { Rarity.Rare, () => BaseLootBoxFindChance + Randomizer.GetRandomNumberBetweenIncludeRange(1, 2) },
            { Rarity.Epic, () => BaseLootBoxFindChance + Randomizer.GetRandomNumberBetweenIncludeRange(3, 4) },
            { Rarity.Legendary, () => BaseLootBoxFindChance + Randomizer.GetRandomNumberBetweenIncludeRange(5, 7) },
            { Rarity.BlackMarket, () => BaseLootBoxFindChance + Randomizer.GetRandomNumberBetweenIncludeRange(8, 10) }
        };

        private const int BaseLootBoxFindChance = 10;

        private int lootBoxFindChance;
    }
}