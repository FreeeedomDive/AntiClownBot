using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders
{
    public class CatWifeBuilder : ItemBuilder
    {
        public CatWifeBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
            IsActive = false;
        }

        public CatWifeBuilder WithRandomAutoTributeChance()
        {
            if (!IsRarityDefined())
            {
                throw new InvalidOperationException("Undefined item rarity");
            }

            autoTributeChance = AutoTributeChanceGenerator[Rarity]();

            return this;
        }

        public CatWife Build() => new()
        {
            Id = Id,
            Price = Price,
            Rarity = Rarity,
            IsActive = IsActive,
            AutoTributeChance = autoTributeChance
        };

        private static readonly Dictionary<Rarity, Func<int>> AutoTributeChanceGenerator = new()
        {
            { Rarity.Common, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(5, 8) },
            { Rarity.Rare, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(9, 12) },
            { Rarity.Epic, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(13, 16) },
            { Rarity.Legendary, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(17, 20) },
            { Rarity.BlackMarket, () => BaseChanceValue + Randomizer.GetRandomNumberBetweenIncludeRange(21, 25) }
        };

        private const int BaseChanceValue = 0;
        private int autoTributeChance;
    }
}