using System;
using System.Collections.Generic;
using AntiClownBotApi.Models.Classes.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class ItemBuilder
    {
        protected Guid Id;
        protected int Price;
        protected Rarity Rarity;

        private readonly Dictionary<Rarity, int> _prices = new()
        {
            {Rarity.Common, 1000},
            {Rarity.Rare, 2500},
            {Rarity.Epic, 4500},
            {Rarity.Legendary, 10000},
            {Rarity.BlackMarket, 20000}
        };

        private static Rarity GenerateRarity() => Randomizer.GetRandomNumberBetween(0, 1000000) switch
        {
            >= 0 and <= 666666 => Rarity.Common,
            > 666666 and <= 930228 => Rarity.Rare,
            > 930228 and <= 999000 => Rarity.Epic,
            > 999000 and <= 999998 => Rarity.Legendary,
            _ => Rarity.BlackMarket
        };

        public ItemBuilder()
        {
            Id = Guid.NewGuid();
        }

        public ItemBuilder WithRarity()
        {
            Rarity = GenerateRarity();

            return this;
        }

        public ItemBuilder WithPrice()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");
            
            Price = _prices[Rarity];

            return this;
        }

        #region Cast to specific item builders

        public CatWifeBuilder AsCatWife() => this as CatWifeBuilder;
        public DogWifeBuilder AsDogWife() => this as DogWifeBuilder;
        public RiceBowlBuilder AsRiceBowl() => this as RiceBowlBuilder;
        public InternetBuilder AsInternet() => this as InternetBuilder;

        #endregion

        #region Static helpers

        protected bool IsRarityDefined() => Enum.IsDefined(Rarity);

        #endregion
    }
}