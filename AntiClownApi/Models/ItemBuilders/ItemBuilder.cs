using System;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class ItemBuilder
    {
        protected Guid Id;
        protected int Price;
        protected Rarity Rarity;
        protected bool IsActive;

        public ItemBuilder()
        {
            Id = Guid.NewGuid();
        }

        public ItemBuilder WithRandomRarity() => WithRarity(Utility.GenerateRarity());

        public ItemBuilder WithRarity(Rarity rarity)
        {
            Rarity = rarity;

            return this;
        }

        public ItemBuilder WithPriceForSelectedRarity()
        {
            if (!IsRarityDefined()) throw new ArgumentException("Item rarity is not defined");

            Price = Utility.Prices[Rarity];

            return this;
        }

        public ItemBuilder WithPrice(int price)
        {
            Price = price;

            return this;
        }

        #region Cast to specific item builders

        public CatWifeBuilder AsCatWife() => new(Id, Rarity, Price);
        public DogWifeBuilder AsDogWife() => new(Id, Rarity, Price);
        public RiceBowlBuilder AsRiceBowl() => new(Id, Rarity, Price);
        public InternetBuilder AsInternet() => new(Id, Rarity, Price);
        public JadeRodBuilder AsJadeRod() => new(Id, Rarity, Price);
        public CommunismBannerBuilder AsCommunismBanner() => new(Id, Rarity, Price);

        #endregion

        #region Static helpers

        protected bool IsRarityDefined() => Enum.IsDefined(Rarity);

        #endregion
    }
}