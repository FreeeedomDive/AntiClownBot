using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Domain.Items.Builders;

namespace IntegrationTests.Inventory;

public class InventoryTestsBase : IntegrationTestsBase
{
    protected static BaseItem GetItem(ItemType itemType)
    {
        BaseItem? item = null;
        while (item == null || item.ItemType != itemType)
        {
            item = new ItemBuilder().BuildRandomItem();
        }

        return item;
    }
}