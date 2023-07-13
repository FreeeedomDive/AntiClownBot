using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public class ItemBuilder
{
    public ItemBuilder()
    {
        Id = Guid.NewGuid();
    }

    public static BaseItem BuildRandomItem(Action<ItemBuilderOptions>? configureBuilderOptions = null)
    {
        var builderOptions = new ItemBuilderOptions();
        configureBuilderOptions?.Invoke(builderOptions);
        var builder = new ItemBuilder
        {
            Rarity = builderOptions.Rarity ?? RarityBuilder.Build(),
            Name = builderOptions.Name ?? ItemNameBuilder.Build(builderOptions.Type),
        };
        builder.Price = builderOptions.CustomPrice ?? PriceBuilder.Build(builder.Rarity);
        return builder.Name switch
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
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private CatWifeBuilder AsCatWife()
    {
        return new CatWifeBuilder(Id, Rarity, Price);
    }

    private DogWifeBuilder AsDogWife()
    {
        return new DogWifeBuilder(Id, Rarity, Price);
    }

    private RiceBowlBuilder AsRiceBowl()
    {
        return new RiceBowlBuilder(Id, Rarity, Price);
    }

    private InternetBuilder AsInternet()
    {
        return new InternetBuilder(Id, Rarity, Price);
    }

    private JadeRodBuilder AsJadeRod()
    {
        return new JadeRodBuilder(Id, Rarity, Price);
    }

    private CommunismBannerBuilder AsCommunismBanner()
    {
        return new CommunismBannerBuilder(Id, Rarity, Price);
    }

    protected bool IsRarityDefined()
    {
        return Enum.IsDefined(Rarity);
    }

    protected Guid Id { get; set; }
    protected int Price { get; set; }
    protected Rarity Rarity { get; set; }
    protected bool IsActive { get; set; }
    protected ItemName Name { get; set; }
}