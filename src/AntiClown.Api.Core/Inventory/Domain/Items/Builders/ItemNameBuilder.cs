using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public static class ItemNameBuilder
{
    public static ItemName Build(ItemType? type = null)
    {
        return type == null
            ? Enum.GetValues<ItemType>()
                  .SelectMany(x => EnumMappings.TypeToNames[x].SelectMany(y => Enumerable.Repeat(y, EnumMappings.TypeToRandomnessWeight[x])))
                  .SelectRandomItem()
            : EnumMappings.TypeToNames[type.Value].SelectRandomItem();
    }
}