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
            { Rarity.Common, () => BaseChanceValue + Randomizer.GetRandomNumberInclusive(5, 8) },
            { Rarity.Rare, () => BaseChanceValue + Randomizer.GetRandomNumberInclusive(9, 12) },
            { Rarity.Epic, () => BaseChanceValue + Randomizer.GetRandomNumberInclusive(13, 16) },
            { Rarity.Legendary, () => BaseChanceValue + Randomizer.GetRandomNumberInclusive(17, 20) },
            { Rarity.BlackMarket, () => BaseChanceValue + Randomizer.GetRandomNumberInclusive(21, 25) }
        };

        private const int BaseChanceValue = 0;
        private int autoTributeChance;
    }
}