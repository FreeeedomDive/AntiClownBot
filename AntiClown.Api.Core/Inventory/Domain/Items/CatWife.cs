using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items;

public class CatWife : BaseItem
{
    public int AutoTributeChance { get; set; }
    public override ItemType ItemType => ItemType.Positive;
}