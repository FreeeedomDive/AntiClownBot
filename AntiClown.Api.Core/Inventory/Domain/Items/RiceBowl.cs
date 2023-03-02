using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items;

public class RiceBowl : BaseItem
{
    public int NegativeRangeExtend { get; set; }
    public int PositiveRangeExtend { get; set; }
    public override ItemType ItemType => ItemType.Positive;
}