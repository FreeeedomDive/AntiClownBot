using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders
{
    public class ItemBuilder
    {
        public ItemBuilder()
        {
            Id = Guid.NewGuid();
        }

        public ItemBuilder WithRandomRarity()
        {
            return WithRarity(RarityBuilder.Build());
        }

        public ItemBuilder WithRarity(Rarity rarity)
        {
            this.Rarity = rarity;

            return this;
        }

        public ItemBuilder WithPriceForSelectedRarity()
        {
            if (!IsRarityDefined())
            {
                throw new InvalidOperationException("Undefined item rarity");
            }

            return WithPrice(PriceBuilder.Build(Rarity));
        }

        public ItemBuilder WithPrice(int price)
        {
            this.Price = price;

            return this;
        }

        public CatWifeBuilder AsCatWife() => new(Id, Rarity, Price);
        public DogWifeBuilder AsDogWife() => new(Id, Rarity, Price);
        public RiceBowlBuilder AsRiceBowl() => new(Id, Rarity, Price);
        public InternetBuilder AsInternet() => new(Id, Rarity, Price);
        public JadeRodBuilder AsJadeRod() => new(Id, Rarity, Price);
        public CommunismBannerBuilder AsCommunismBanner() => new(Id, Rarity, Price);

        public BaseItem BuildRandomItem()
        {
            var builder = new ItemBuilder()
                .WithRandomRarity()
                .WithPriceForSelectedRarity();
            var itemName = ItemNameBuilder.Build();
            return itemName switch
            {
                ItemName.CatWife => builder
                    .AsCatWife()
                    .WithRandomAutoTributeChance()
                    .Build(),
                ItemName.CommunismBanner => builder
                    .AsCommunismBanner()
                    .WithRandomDistributedStats()
                    .Build(),
                ItemName.DogWife => builder
                    .AsDogWife()
                    .WithRandomLootBoxFindChance()
                    .Build(),
                ItemName.Internet => builder.AsInternet()
                    .WithRandomCooldownReducePercent()
                    .WithRandomCooldownReduceTries()
                    .WithRandomCooldownReduceChance()
                    .WithRandomDistributedStats()
                    .Build(),
                ItemName.JadeRod => builder
                    .AsJadeRod()
                    .WithRandomDistributedStats()
                    .Build(),
                ItemName.RiceBowl => builder
                    .AsRiceBowl()
                    .WithRandomTributeIncrease()
                    .WithRandomTributeDecrease()
                    .WithRandomDistributedStats()
                    .Build(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected bool IsRarityDefined() => Enum.IsDefined(Rarity);

        protected Guid Id;
        protected int Price;
        protected Rarity Rarity;
        protected bool IsActive;
    }
}