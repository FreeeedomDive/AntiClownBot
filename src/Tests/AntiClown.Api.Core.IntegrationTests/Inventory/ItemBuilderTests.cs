using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Domain.Items.Builders;
using FluentAssertions;

namespace AntiClown.Api.Core.IntegrationTests.Inventory;

public class ItemBuilderTests : IntegrationTestsBase
{
    [Test]
    public void ItemBuilder_Should_BuildAllTypes([Values] ItemType itemType)
    {
        ProcessItemBuildingWithPropertyCheck(item => item.ItemType == itemType);
    }

    [Test]
    public void ItemBuilder_Should_BuildAllNames([Values] ItemName itemName)
    {
        ProcessItemBuildingWithPropertyCheck(item => item.ItemName == itemName);
    }

    [Test]
    public void ItemBuilder_Should_BuildAllRarities([Values] Rarity rarity)
    {
        ProcessItemBuildingWithPropertyCheck(item => item.Rarity == rarity);
    }

    [Test]
    public void PositiveItem_Should_BeInactiveByDefault()
    {
        var positiveItem = ItemBuilder.BuildRandomItem(config => config.Type = ItemType.Positive);
        positiveItem.IsActive.Should().BeFalse();
    }

    [Test]
    public void NegativeItem_Should_BeActiveByDefault()
    {
        var negativeItem = ItemBuilder.BuildRandomItem(config => config.Type = ItemType.Negative);
        negativeItem.IsActive.Should().BeTrue();
    }

    [Test]
    public void InspectRarityDistributions()
    {
        var counter = new Dictionary<Rarity, int>
        {
            { Rarity.Common, 0 },
            { Rarity.Rare, 0 },
            { Rarity.Epic, 0 },
            { Rarity.Legendary, 0 },
            { Rarity.BlackMarket, 0 },
        };
        for (var i = 0; i < TriesToBuild; i++)
        {
            var item = ItemBuilder.BuildRandomItem();
            counter[item.Rarity]++;
        }

        Console.WriteLine(string.Join("\n", counter.Select(kv => $"{kv.Key}: {kv.Value}")));
    }

    /// <summary>
    ///     Функция для тестов на то, что в рандомайзере могут попасть все разновидности предметов
    /// </summary>
    /// <param name="propertyCheckFunc"></param>
    private static void ProcessItemBuildingWithPropertyCheck(Func<BaseItem, bool> propertyCheckFunc)
    {
        for (var i = 1; i <= TriesToBuild; i++)
        {
            var item = ItemBuilder.BuildRandomItem();
            if (!propertyCheckFunc(item))
            {
                continue;
            }

            Console.WriteLine($"Required item was built in {i} iterations");
            Assert.Pass();
        }

        Assert.Fail($"{nameof(ItemBuilder)} did not build item with required parameter value");
    }

    private const int TriesToBuild = 1_500_000;
}